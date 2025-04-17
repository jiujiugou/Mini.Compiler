
namespace Mini.Compiler.CodeAnalysis.Syntax
{
    internal class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken open, ExpressionSyntax expression, SyntaxToken close)
        {
            OpenParenthesisToken = open;
            InnerExpression = expression;
            CloseParenthesisToken = close;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax InnerExpression { get; }    // 注意这里可能叫 InnerExpression
        public SyntaxToken CloseParenthesisToken { get; }
    }
}