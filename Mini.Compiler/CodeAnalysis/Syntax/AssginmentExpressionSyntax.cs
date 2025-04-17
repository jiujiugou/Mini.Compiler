
namespace Mini.Compiler.CodeAnalysis.Syntax;

public sealed class AssginmentExpressionSyntax : ExpressionSyntax
{
    public AssginmentExpressionSyntax(SyntaxToken identifier, SyntaxToken equals, ExpressionSyntax expression)
    {
        Identifier = identifier;
        Equals = equals;
        Expression = expression;
    }
    public SyntaxToken Identifier { get; }
    public SyntaxToken Equals { get; }
    public ExpressionSyntax Expression { get; }
    public override SyntaxKind Kind => SyntaxKind.AssginmentExpression;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return Equals;
        yield return Expression;
    }
}
