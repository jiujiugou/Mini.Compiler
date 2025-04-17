using Mini.Compiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText text, ImmutableArray<Diagnostics> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileToken = endOfFileToken;
            Text = text;
        }
        public ImmutableArray<Diagnostics> Diagnostics { get; }
        public ExpressionSyntax Root { get; } 
        public SyntaxToken EndOfFileToken { get; }
        public SourceText Text { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        private static SyntaxTree Parse(SourceText sourceText)
        {
            var parser = new Parser(sourceText);
            return parser.Parse();
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
