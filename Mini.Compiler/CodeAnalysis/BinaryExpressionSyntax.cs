namespace Mini.Compiler
{
    sealed class BinaryExpressionSyntax : ExpressionSynax
    {
        public BinaryExpressionSyntax(ExpressionSynax left, SyntaxToken operatorToken, ExpressionSynax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }  
        public ExpressionSynax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSynax Right { get; }
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}
