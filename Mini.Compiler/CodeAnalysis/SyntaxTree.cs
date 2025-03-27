namespace Mini.Compiler.CodeAnalysis
{
    sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSynax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }
        public IEnumerable<string> Diagnostics { get; }
        public ExpressionSynax Root { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
}
