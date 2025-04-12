using Mini.Compiler.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;

namespace Mini.Compiler.CodeAnalysis.Binding
{

    internal sealed class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => _diagnostics;

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
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
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
                _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Kind}' is not defined for type {operand.Type}.");
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
                _diagnostics.Add($"Binary operator '{syntax.OperatorToken.Kind}' is not defined for types {left.Type} and {right.Type}.");
                return new BoundErrorExpression();
            }
            return new BoundBinaryExpression(left, opKind, right);
        }

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
        }
    }
}
