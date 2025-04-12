namespace Mini.Compiler.CodeAnalysis.Syntax
{
    class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();
        public Lexer(string text)
        {
            _text = text;
        }
        public IEnumerable<string> Diagnostics => _diagnostics;
        
        private char Current => Peek(0);

        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
            {
                return '\0';
            }
            return _text[index];
        }
        private void Next()
        {
            _position++;
        }
        public SyntaxToken Lex()
        {
            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null!);
            }
            if (char.IsDigit(Current))
            {
                var start = _position;
                while (char.IsDigit(Current))
                {
                    Next();
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"ERROR: The number {_text} is not a valid Int32");
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }
            if (char.IsWhiteSpace(Current))
            {
                var start = _position;
                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null!);
            }
            if (char.IsLetter(Current)) 
            {
                var start = _position;
                while (char.IsLetterOrDigit(Current))
                {
                    Next();
                }
                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind =SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null!);
            }

            SyntaxToken token;
            switch (Current)
            {
                case '+': token = new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null!); break;
                case '-': token = new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null!); break;
                case '*': token = new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null!); break;
                case '/': token = new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null!); break;
                case '(': token = new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null!); break;
                case ')': token = new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null!); break;
                case '=':
                    if (Lookahead == '=')
                    {
                        token = new SyntaxToken(SyntaxKind.EqualsEqualsToken, _position, "==", null!);
                        _position += 2;
                    }
                    else
                    {
                        token = new SyntaxToken(SyntaxKind.EqualsToken, _position++, "=", null!);
                    }
                    break;
                case '!':
                    if (Lookahead == '=')
                    {
                        token = new SyntaxToken(SyntaxKind.BangEqualToken, _position, "!=", null!);
                        _position += 2;
                    }
                    else
                    {
                        token = new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null!);
                    }
                    break;
                case '&': 
                    if(Lookahead == '&')
                    {
                        token = new SyntaxToken(SyntaxKind.AmpersandEqualToken, _position, "&&", null!);
                        _position += 2;
                    }
                    else
                    {
                        token = new SyntaxToken(SyntaxKind.AmpersandToken, _position++, "&", null!);
                    }
                    break;
                case '|':
                    if(Lookahead == '|')
                    {
                        token = new SyntaxToken(SyntaxKind.PipeEqualToken, _position, "||", null!);
                        _position += 2;
                    }
                    else
                    {
                        token = new SyntaxToken(SyntaxKind.PipeToken, _position++, "|", null!);
                    }
                    break;
                default:
                    _diagnostics.Add($"ERROR: bad character input: '{Current}'");
                    token = new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null!);
                    break;
            }
            return token;
        }
    }
}
