﻿namespace Mini.Compiler
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
        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                {
                    return '\0';
                }
                return _text[_position];
            }
        }
        private void Next()
        {
            _position++;
        }
        public SyntaxToken NextToken()
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
            SyntaxToken token;
            switch (Current)
            {
                case '+': token = new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null!); break;
                case '-': token = new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null!); break;
                case '*': token = new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null!); break;
                case '/': token = new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null!); break;
                case '(': token = new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null!); break;
                case ')': token = new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null!); break;
                default:
                    _diagnostics.Add($"ERROR: bad character input: '{Current}'");
                    token = new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null!);
                    break;
            }
            return token;
        }
    }
}
