namespace Mini.Compiler.CodeAnalysis
{
    class Evaluator
    {
        private readonly ExpressionSynax _root;
        public Evaluator(ExpressionSynax root)
        {
            _root = root;
        }
        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSynax root)
        {
            if (root is NumberExpressionSynax n)
            {
                return (int)n.NumberToken.Value;
            }
            if (root is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);
                if (u.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return operand;
                else if (u.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return -operand;
                throw new Exception($"Unexpected unary operator {u.OperatorToken.Kind}");
            }
            if (root is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);
                if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                {
                    if (right == 0)
                        throw new DivideByZeroException("Cannot divide by zero.");
                    return left / right;
                }
                throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }
            throw new Exception($"Unexpected node {root.Kind}");
        }

    }
}
