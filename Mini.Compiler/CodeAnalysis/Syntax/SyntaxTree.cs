namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(DiagnosticBag diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileToken = endOfFileToken;
        }
        public DiagnosticBag Diagnostics { get; }
        public ExpressionSyntax Root { get; } 
        public SyntaxToken EndOfFileToken { get; }
    }
}
