using Mini.Compiler.CodeAnalysis.Syntax;
using Mini.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new();
        private BoundScope _scope;

        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScope(previous);
            var binder = new Binder(parentScope);
            var expression = binder.BindStatement(syntax.Statement);
            var variables = binder._scope.GetDeclearedVariables();
            var diagnostics = binder._diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
            }

            return new BoundGlobalScope(previous, diagnostics, variables, expression);
        }

        private static BoundScope CreateParentScope(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.BoundGlobal;
            }

            BoundScope parent = null;
            while (stack.Count > 0)
            {
                var global = stack.Pop();
                var scope = new BoundScope(parent);

                foreach (var variable in global.VariableSymbols)
                {
                    scope.DeclareVariable(variable.Name, variable);
                }

                parent = scope;
            }

            return parent;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxKind.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                case SyntaxKind.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, typeof(bool));
            var body = BindStatement(syntax.Body);
            return new BoundWhileStatement(condition, body);
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var lowerBound = BindExpression(syntax.LowerBound, typeof(int));
            var upperBound = BindExpression(syntax.UpperBound, typeof(int));
            var name = syntax.Identifier.Text;
            var variable = new VariableSymbol(name,true, typeof(int));
            _scope=new BoundScope(_scope);
            if (!_scope.TryDeclare(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
            }
            var body = BindStatement(syntax.Body);
            _scope = _scope.Parent;
            return new BoundForStatement(variable, lowerBound, upperBound, body);
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
            var initializer = BindExpression(syntax.Initializer);
            var variable = new VariableSymbol(name, isReadOnly, initializer.Type);
            if (!_scope.TryDeclare(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(syntax.Initializer.Span, name);
            }
            return new BoundVariableDeclaration(variable, initializer);
        }
        
        private BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssginmentExpression:
                    return BindAssginmentExpression((AssginmentExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }
        public BoundExpression BindExpression(ExpressionSyntax syntax, Type type)
        {
            var boundExpression = BindExpression(syntax);
            if (boundExpression.Type != type)
            {
                _diagnostics.ReportCannotConvert(syntax.Span, boundExpression.Type, type);
                return new BoundErrorExpression();
            }
            return boundExpression;
        }
        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            // 1. 绑定并做类型检查
            var condition = BindExpression(syntax.Condition, typeof(bool));

            // 2. 绑定 then 分支
            var thenStmt = BindStatement(syntax.ThenStatemrnt);

            // 3. 绑定 else 分支（如果存在）
            BoundStatement? elseStmt = null;
            if (syntax.ElseClause != null)
                elseStmt = BindStatement(syntax.ElseClause.Statement);

            // 4. 返回绑定后的 If 语句
            return new BoundIfStatement(condition, thenStmt, elseStmt);
        }


        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var outer = _scope;
            _scope = new BoundScope(outer);
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            foreach (var statement in syntax.Statements)
            {
                var boundStatement = BindStatement(statement);
                if (boundStatement != null)
                {
                    builder.Add(boundStatement);
                }
            }
            _scope = outer;
            return new BoundBlockStatement(builder.ToImmutable());
        }

        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);
            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.InnerExpression);
        }

        private BoundExpression BindAssginmentExpression(AssginmentExpressionSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var boundExpression = BindExpression(syntax.Expression);
            // 首次声明时加入，之后赋值时只更新，不报错
            if (!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefineName(syntax.Identifier.Span, name);
                return boundExpression;
            }
            if(variable.IsReadOnly)
            {
                _diagnostics.ReportCannotAssign(syntax.Identifier.Span, name);
            }
            if (boundExpression.Type != variable.Type)
            {
                _diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }
            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.identifierToken.Text;
            if (!_scope.TryLookupVariable(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.identifierToken.Span, name);
                return new BoundErrorExpression();
            }
            return new BoundNameExpression(variable);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            return new BoundLiteralExpression(syntax.Value!);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var operand = BindExpression(syntax.Operand);
            var opKind = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, operand.Type);
            if (opKind == null)
            {
                _diagnostics.ReportUnDefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, operand.Type);
                return new BoundErrorExpression();
            }
            return new BoundUnaryExpression(operand, opKind);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var left = BindExpression(syntax.Left);
            var right = BindExpression(syntax.Right);
            var opKind = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);
            //// —— 加入这段调试输出 —— //
            //if (opKind == null)
            //{
            //    Console.Error.WriteLine(
            //        $"[Binder] 无法为 “{syntax.OperatorToken.Text}” " +
            //        $"在类型 ({left.Type.Name}, {right.Type.Name}) 上绑定操作符");
            //}
            //else
            //{
            //    Console.Error.WriteLine(
            //        $"[Binder] 为 “{syntax.OperatorToken.Text}” 绑定到 {opKind.OperatorKind}，结果类型 {opKind.ResultType.Name}");
            //}
            //// —— 调试结束 —— //
            if (opKind == null)
            {
                _diagnostics.ReportUnDefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, left.Type, right.Type);
                return new BoundErrorExpression();
            }
            return new BoundBinaryExpression(left, opKind, right);
        }

        private object? GetDefaultValue(Type type)
        {
            if (type == typeof(int)) return 0;
            if (type == typeof(bool)) return false;
            return null;
        }
    }
}
