namespace Mini.Compiler
{
    sealed class UnaryExpressionSyntax : ExpressionSynax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSynax operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public SyntaxToken OperatorToken { get; }
        public ExpressionSynax Operand { get; }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }
}
