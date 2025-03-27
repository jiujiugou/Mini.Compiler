namespace Mini.Compiler
{
    sealed class NumberExpressionSynax : ExpressionSynax
    {
        public NumberExpressionSynax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }
        public override SyntaxKind Kind => SyntaxKind.NumberToken;
        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }
}
