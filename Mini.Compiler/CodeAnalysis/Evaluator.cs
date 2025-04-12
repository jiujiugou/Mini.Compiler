using Mini.Compiler.CodeAnalysis.Binding;
using Mini.Compiler.CodeAnalysis.Syntax;
using System;

namespace Mini.Compiler.CodeAnalysis
{
    class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression root)
        {
            switch (root)
            {
                case BoundLiteralExpression literal:
                    return literal.Value;

                case BoundUnaryExpression unary:
                    var operand = EvaluateExpression(unary.Operand);
                    switch (unary.Operator.OperatorKind)
                    {
                        case BoundUnaryOperatorKind.Identity:
                            return operand;
                        case BoundUnaryOperatorKind.Negation:
                            return -(int)operand;
                        case BoundUnaryOperatorKind.LogicalNegation:
                            return !(bool)operand;
                        default:
                            throw new Exception($"Unexpected unary operator {unary.Operator.OperatorKind}");
                    }

                case BoundBinaryExpression binary:
                    var left = EvaluateExpression(binary.Left);
                    var right = EvaluateExpression(binary.Right);
                    switch (binary.Operator.OperatorKind)
                    {
                        case BoundBinaryOperatorKind.Addition:
                            return (int)left + (int)right;
                        case BoundBinaryOperatorKind.Subtraction:
                            return (int)left - (int)right;
                        case BoundBinaryOperatorKind.Multiplication:
                            return (int)left * (int)right;
                        case BoundBinaryOperatorKind.Division:
                            if ((int)right == 0)
                                throw new DivideByZeroException("Cannot divide by zero.");
                            return (int)left / (int)right;
                        case BoundBinaryOperatorKind.LogicalAnd:
                            return (bool)left && (bool)right;
                        case BoundBinaryOperatorKind.LogicalOr:
                            return (bool)left || (bool)right;
                        case BoundBinaryOperatorKind.Equals:
                            return Equals(left, right);
                        case BoundBinaryOperatorKind.NotEquals:
                            return !Equals(left, right);
                        default:
                            throw new Exception($"Unexpected binary operator {binary.Operator.OperatorKind}");
                    }

                default:
                    throw new Exception($"Unexpected node kind: {root.Kind}");
            }
        }
    }
}
