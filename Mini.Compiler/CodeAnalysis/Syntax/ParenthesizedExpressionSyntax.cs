
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

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            // 先返回 '('
            yield return OpenParenthesisToken;
            // 然后返回括号内的表达式
            yield return InnerExpression;
            // 最后返回 ')'
            yield return CloseParenthesisToken;
        }
    }
}