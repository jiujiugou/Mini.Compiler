using Mini.Compiler.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mini.Compiler.CodeAnalysis.Binding
{

    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new();
        private Dictionary<string, object> variables;

        public Binder(Dictionary<string, object> variables)
        {
            this.variables = variables;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.ParenthesizedExpression:
                    // 去掉括号，绑定内部表达式
                    return BindExpression(((ParenthesizedExpressionSyntax)syntax).InnerExpression);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssginmentExpression:
                    return BindAssginmentExpression((AssginmentExpressionSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindAssginmentExpression(AssginmentExpressionSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var boundexpression = BindExpression(syntax.Expression);
            var type = boundexpression.Type;
            if (!variables.ContainsKey(name))
            {
                variables[name] = GetDefaultValue(type)!;
            }
            var existingName = variables[name];
            if (existingName.GetType() != type)
            {
                _diagnostics.ReportNotAssginment(syntax.Identifier.Span,existingName.GetType(),type);
                return new BoundErrorExpression();
            }
            return new BoundAssignmentExpression(name, boundexpression);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name= syntax.identifierToken.Text;
            if (!variables.TryGetValue(name, out var value))
            {
                _diagnostics.ReportUndefinedName(syntax.identifierToken.Span, name);
                return new BoundErrorExpression();
            }
            var type = value?.GetType()??typeof(object);
            return new BoundNameExpression(name, type,value);
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
                _diagnostics.ReportUnDefinedUnaryOperator(syntax.OperatorToken.Span,syntax.OperatorToken.Text,operand.Type);
                return new BoundErrorExpression();
            }
            return new BoundUnaryExpression(operand, opKind);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var left = BindExpression(syntax.Left);
            var right = BindExpression(syntax.Right);
            var opKind = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, left.Type, right.Type);
            if (opKind == null)
            {
                _diagnostics.ReportUnDefinedBinaryOperator(syntax.OperatorToken.Span,syntax.OperatorToken.Text,left.Type,right.Type);
                return new BoundErrorExpression();
            }
            return new BoundBinaryExpression(left, opKind, right);
        }
        private object? GetDefaultValue(Type type)
        {
            if (type == typeof(int)) return 0;
            if (type == typeof(bool)) return false;
            return null!;
        }
        /*
        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type type)
        {
            if (type == typeof(int))
            {
                return kind switch
                {
                    SyntaxKind.PlusToken => BoundUnaryOperatorKind.Identity,
                    SyntaxKind.MinusToken => BoundUnaryOperatorKind.Negation,
                    _ => null
                };
            }
            if (type == typeof(bool))
            {
                return kind == SyntaxKind.BangToken
                    ? BoundUnaryOperatorKind.LogicalNegation
                    : null;
            }
            return null;
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType)
        {
            if (leftType == typeof(int) && rightType == typeof(int))
            {
                return kind switch
                {
                    SyntaxKind.PlusToken => BoundBinaryOperatorKind.Addition,
                    SyntaxKind.MinusToken => BoundBinaryOperatorKind.Subtraction,
                    SyntaxKind.StarToken => BoundBinaryOperatorKind.Multiplication,
                    SyntaxKind.SlashToken => BoundBinaryOperatorKind.Division,
                    _ => null
                };
            }
            if (leftType == typeof(bool) && rightType == typeof(bool))
            {
                return kind switch
                {
                    SyntaxKind.AmpersandToken => BoundBinaryOperatorKind.LogicalAnd,
                    SyntaxKind.PipeToken => BoundBinaryOperatorKind.LogicalOr,
                    _ => null
                };
            }
            return null;
        }*/
    }
}
