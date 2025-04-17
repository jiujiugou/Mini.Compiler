

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private DiagnosticBag _diagnostics = new();
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
        public DiagnosticBag Diagnostics => _diagnostics;
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
            _diagnostics.ReportUnexpectedToken(Current.Span,Current.Kind,kind);
            return new SyntaxToken(kind, Current.Postion, kind.ToString(), null!);
        }
        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }
        private ExpressionSyntax ParseAssignmentExpression()
        {
            if (Peek(0).Kind == SyntaxKind.IdentifierToken
                && Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssginmentExpressionSyntax(identifierToken, operatorToken, right);
            }
            else
            {
                return ParseExpression();
            }
        }
        public ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            // 优先处理赋值表达式（a = b = 3）
            if (Peek(0).Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = NextToken();
                var equalsToken = NextToken();
                var right = ParseExpression(); // 不递归 ParseAssignmentExpression 以避免优先级混乱
                return new AssginmentExpressionSyntax(identifierToken, equalsToken, right);
            }
            // 处理一元运算符
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
            // 处理二元运算符
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
                case SyntaxKind.IdentifierToken:
                    {
                        var identifierToken = NextToken();
                        return new NameExpressionSyntax(identifierToken);
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
