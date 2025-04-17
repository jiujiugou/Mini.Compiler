using Mini.Compiler.CodeAnalysis.Syntax;

namespace Mini.Compiler.Test
{
    public class LexerTest
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }
        [Theory]
        [MemberData(nameof(GetTokenPairData))]
        public void Lexer_Lexes_TokenPairs(SyntaxKind t1kind,string t1text,SyntaxKind t2kind,string t2text)
        {
            var text = t1text + t2text;
            var tokens = SyntaxTree.ParseTokens(text).ToArray();
            Assert.Equal(2, tokens.Length);
            Assert.Equal(t1kind, tokens[0].Kind);
            Assert.Equal(t1text, tokens[0].Text);
            Assert.Equal(t2kind, tokens[1].Kind);
            Assert.Equal(t2text, tokens[1].Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var t in GetTokens())
            {
                yield return new object[] { t.kind, t.text };
            }
        }
        public static IEnumerable<object[]> GetTokenPairData()=> 
            GetTokenPairs().Select(tp => new object[] {tp.t1kind,tp.t1text,tp.t2kind,tp.t2text });

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            return new[]
            {
                (SyntaxKind.NumberToken, "123"),
                //(SyntaxKind.WhiteSpaceToken, " "),
                (SyntaxKind.PlusToken, "+"),
                (SyntaxKind.MinusToken, "-"),
                (SyntaxKind.StarToken, "*"),
                (SyntaxKind.SlashToken, "/"),
                (SyntaxKind.OpenParenthesisToken, "("),
                (SyntaxKind.CloseParenthesisToken, ")"),
                //(SyntaxKind.EndOfFileToken, ""),
                (SyntaxKind.IdentifierToken, "abc"),
                //(SyntaxKind.AmpersandToken, "&"),
                (SyntaxKind.PipeToken, "|"),
                (SyntaxKind.BangToken, "!"),
                //(SyntaxKind.AmpersandEqualToken, "&&"),
                //(SyntaxKind.PipeEqualToken, "||"),
                //(SyntaxKind.BangEqualToken, "!="),
                //(SyntaxKind.EqualsEqualsToken, "=="),
                (SyntaxKind.EqualsToken, "=")
            };
        }

        private static IEnumerable<(SyntaxKind t1kind, string t1text, SyntaxKind t2kind, string t2text)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if(!RequireSeparator(t1.kind,t2.kind))
                        yield return (t1.kind,t1.text, t2.kind, t2.text);
                }
            }
        }

        private static bool RequireSeparator(SyntaxKind t1kind, SyntaxKind t2kind)
        {
            var t1keyword = t1kind.ToString().EndsWith("Keyword");
            var t2keyword = t2kind.ToString().EndsWith("Keyword");
            if (t1keyword && t2keyword)
            {
                return true;
            }
            if (t1keyword && t2kind == SyntaxKind.IdentifierToken)
            {
                return true;
            }
            if (t2keyword && t1kind == SyntaxKind.IdentifierToken)
            {
                return true;
            }
            if(t1kind==SyntaxKind.NumberToken && t2kind == SyntaxKind.NumberToken)
            {
                return true;
            }
            if(t1kind == SyntaxKind.BangToken && t2kind == SyntaxKind.EqualsToken)
            {
                return true;
            }
            if (t1kind==SyntaxKind.BadToken&&t2kind==SyntaxKind.EqualsToken)
            {
                return true;
            }
            if (t1kind == SyntaxKind.EqualsToken && t2kind == SyntaxKind.EqualsToken)
            {
                return true;
            }
            if(t1kind == SyntaxKind.IdentifierToken && t2kind == SyntaxKind.IdentifierToken)
            {
                return true;
            }
            if(t1kind == SyntaxKind.EqualsToken && t2kind == SyntaxKind.EqualsEqualsToken)
            {
                return true;
            }
            if(SyntaxKind.PipeToken == t1kind && SyntaxKind.PipeToken == t2kind)
            {
                return true;
            }
            if (t1kind == SyntaxKind.IdentifierToken && t2kind == SyntaxKind.NumberToken)
            {
                return true;
            }
            return false;
        }
    }
}