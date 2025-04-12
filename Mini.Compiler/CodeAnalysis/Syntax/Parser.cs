namespace Mini.Compiler.CodeAnalysis.Syntax
{
    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private List<string> _diagnostics = new List<string>();
        private int _position;
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.WhiteSpaceToken && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);
            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }
        public IEnumerable<string> Diagnostics => _diagnostics;
        private SyntaxToken Peek(int offest)
        {
            var index = _position + offest;
            if (index >= _tokens.Length)
            {
                return _tokens[_tokens.Length - 1];
            }
            return _tokens[index];
        }
        private SyntaxToken Current => Peek(0);
        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }
        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
            {
                return NextToken();
            }
            _diagnostics.Add($"ERROR: unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Postion, kind.ToString(), null!);
        }
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }
        public ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(Current.Kind);

            if (unaryPrecedence > 0)
            {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = SyntaxFacts.GetBinaryOperatorPrecedence(Current.Kind);
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }


        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    {
                        var left = NextToken();
                        var expression = ParseExpression();
                        var right = Match(SyntaxKind.CloseParenthesisToken);
                        return new ParenthesizedExpressionSyntax(left, expression, right);
                    }
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueKeyword:
                    {
                        var keyToken = NextToken();
                        var value = keyToken.Kind == SyntaxKind.TrueKeyword;
                        return new LiteralExpressionSyntax(keyToken, value);
                    }
                default:
                    {
                        var numberToken = Match(SyntaxKind.NumberToken);
                        var value = numberToken.Value is int i ? i : 0;
                        return new LiteralExpressionSyntax(numberToken, value);
                    }
            }
            
        }
    }
}
