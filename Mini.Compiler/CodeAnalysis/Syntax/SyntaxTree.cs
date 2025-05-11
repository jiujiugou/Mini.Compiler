using Mini.Compiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText text)
        {
            var prase=new Parser(text);
            var root = prase.CompilationUnit();
            var diagnostics = prase.Diagnostics.ToImmutableArray();
            Root = root;
            Text = text;
            Diagnostics = diagnostics;
        }
        public ImmutableArray<Diagnostics> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }
        public SourceText Text { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        private static SyntaxTree Parse(SourceText sourceText)
        {
            return new SyntaxTree(sourceText);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
        {
            var lex = new Lexer(text);
            while (true)
            {
                var token = lex.Lex();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                {
                    break;
                }
                yield return token;
            }
        }
    }
}
