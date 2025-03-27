

using Mini.Compiler.CodeAnalysis;

namespace Mini.Compiler
{
    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private List<string> _diagnostics= new List<string>();
        private int _position;
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();
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
            return new SyntaxToken(kind, Current.Postion, null!, null!);
        }
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }
        public ExpressionSynax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSynax left;
            var unaryPrecedence = GetUnaryOperatorPrecedence(Current.Kind);

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
                var precedence = GetBinaryOperatorPrecedence(Current.Kind);
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private int GetBinaryOperatorPrecedence(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.StarToken or SyntaxKind.SlashToken => 2,
                SyntaxKind.PlusToken or SyntaxKind.MinusToken => 1,
                _ => 0,
            };
        }
        private int GetUnaryOperatorPrecedence(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.MinusToken => 3, // 负号优先级最高
                _ => 0,
            };
        }
        private ExpressionSynax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                NextToken(); // 跳过 '('
                var expression = ParseExpression();
                Match(SyntaxKind.CloseParenthesisToken);
                return expression;
            }
            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSynax(numberToken);
        }

    }
}
