using Mini.Compiler.CodeAnalysis.Text;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    class Lexer
    {
        private readonly SourceText _text;
        private int _position;
        private DiagnosticBag _diagnostics = new DiagnosticBag();

        public Lexer(SourceText text)
        {
            _text = text;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
                return '\0';
            return _text[index];
        }

        private void Next() => _position++;

        public SyntaxToken Lex()
        {
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null!);

            // Handle CRLF
            if (Current == '\r' && Lookahead == '\n')
            {
                var start = _position;
                _position += 2;
                return new SyntaxToken(SyntaxKind.NewLineToken, start, "\r\n", null!);
            }

            // Handle single-character newlines
            if (Current == '\n' || Current == '\r')
            {
                var start = _position;
                var text = Current.ToString();
                Next();
                return new SyntaxToken(SyntaxKind.NewLineToken, start, text, null!);
            }

            // Handle spaces and tabs
            if (Current == ' ' || Current == '\t')
            {
                var start = _position;
                while (Current == ' ' || Current == '\t')
                    Next();
                var length = _position - start;
                var text = _text.ToString(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null!);
            }

            // Number literal
            if (char.IsDigit(Current))
            {
                var start = _position;
                while (char.IsDigit(Current))
                    Next();
                var length = _position - start;
                var text = _text.ToString(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(int));
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            // Identifier or keyword
            if (char.IsLetter(Current))
            {
                var start = _position;
                while (char.IsLetterOrDigit(Current))
                    Next();
                var length = _position - start;
                var text = _text.ToString(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, start, text, null!);
            }

            // Punctuation and operators
            SyntaxToken token;
            switch (Current)
            {
                case '+': token = new SyntaxToken(SyntaxKind.PlusToken, _position, "+", null!); Next(); break;
                case '-': token = new SyntaxToken(SyntaxKind.MinusToken, _position, "-", null!); Next(); break;
                case '*': token = new SyntaxToken(SyntaxKind.StarToken, _position, "*", null!); Next(); break;
                case '/': token = new SyntaxToken(SyntaxKind.SlashToken, _position, "/", null!); Next(); break;
                case '(': token = new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position, "(", null!); Next(); break;
                case ')': token = new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position, ")", null!); Next(); break;
                case '{': token = new SyntaxToken(SyntaxKind.OpenBraceToken, _position, "{", null!); Next(); break;
                case '}': token = new SyntaxToken(SyntaxKind.CloseBraceToken, _position, "}", null!); Next(); break;
                case ';': token = new SyntaxToken(SyntaxKind.SemicolonToken, _position, ";", null!); Next(); break;
                case '<':
                    if(Lookahead == '=')
                    {
                        token = new SyntaxToken(SyntaxKind.LessThanEqualsToken, _position, "<=", null!);
                        _position += 2;
                    }
                    else
                    {
                        token = new SyntaxToken(SyntaxKind.LessThanToken, _position, "<", null!);
                        Next();
                    }
                    break;
                case '>':
                    if (Lookahead == '=')
                    {
                        token = new SyntaxToken(SyntaxKind.GreaterThanEqualsToken, _position, ">=", null!);
                        _position += 2;
                    }
                    else
                    {
                        token = new SyntaxToken(SyntaxKind.GreaterThanToken, _position, ">", null!);
                        Next();
                    }
                    break;
                case '=' when Lookahead == '=':
                    token = new SyntaxToken(SyntaxKind.EqualsEqualsToken, _position, "==", null!);
                    _position += 2;
                    break;
                case '=':
                    token = new SyntaxToken(SyntaxKind.EqualsToken, _position, "=", null!);
                    Next();
                    break;
                case '!':
                    if (Lookahead == '=')
                    {
                        token = new SyntaxToken(SyntaxKind.BangEqualToken, _position, "!=", null!);
                        _position += 2;
                    }
                    else
                    {
                        token = new SyntaxToken(SyntaxKind.BangToken, _position, "!", null!);
                        Next();
                    }
                    break;
                case '&' when Lookahead == '&':
                    token = new SyntaxToken(SyntaxKind.AmpersandEqualToken, _position, "&&", null!);
                    _position += 2;
                    break;
                case '&':
                    token = new SyntaxToken(SyntaxKind.AmpersandToken, _position, "&", null!);
                    Next();
                    break;
                case '|' when Lookahead == '|':
                    token = new SyntaxToken(SyntaxKind.PipePipeToken, _position, "||", null!);
                    _position += 2;
                    break;
                case '|':
                    token = new SyntaxToken(SyntaxKind.PipeToken, _position, "|", null!);
                    Next();
                    break;
                default:
                    _diagnostics.ReportBadCharacter(_position, Current);
                    token = new SyntaxToken(SyntaxKind.BadToken, _position, _text.ToString(_position, 1), null!);
                    Next();
                    break;
            }
            return token;
        }
    }
}
