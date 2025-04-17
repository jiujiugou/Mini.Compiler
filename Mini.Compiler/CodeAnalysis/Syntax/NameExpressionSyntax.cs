
namespace Mini.Compiler.CodeAnalysis.Syntax;

public sealed class NameExpressionSyntax : ExpressionSyntax
{
    public NameExpressionSyntax(SyntaxToken identifierToken)
    {
        this.identifierToken = identifierToken;
    }
    public override SyntaxKind Kind => SyntaxKind.NameExpression;
    public SyntaxToken identifierToken { get; }

}
