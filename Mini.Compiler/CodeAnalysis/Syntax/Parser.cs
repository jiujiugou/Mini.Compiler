using Mini.Compiler.CodeAnalysis.Text;
using System.Collections.Immutable;
using Mini.Compiler.CodeAnalysis.Syntax;
using System.Timers;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    class Parser
    {
        private readonly ImmutableArray<SyntaxToken> _tokens;
        private DiagnosticBag _diagnostics = new();
        private int _position;
        private readonly SourceText _text;
        public Parser(SourceText text)
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
            _tokens = tokens.ToImmutableArray();
            _diagnostics.AddRange(lexer.Diagnostics);
            _text = text;
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
                return NextToken();
            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            // 合成一个占位 token
            NextToken();
            var text = Current.Text;
            var value = Current.Value;
            return new SyntaxToken(kind, Current.Postion, text, value);
        }

        public CompilationUnitSyntax CompilationUnit()
        {
            var expression = ParseStatement();
            while(Current.Kind == SyntaxKind.NewLineToken)
            {
                NextToken();
            }
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax( endOfFileToken, expression);
        }
        public StatementSyntax ParseStatement()
        {
            while (Current.Kind == SyntaxKind.NewLineToken)
            {
                NextToken();
            }
            if (Current.Kind == SyntaxKind.OpenBraceToken)
            {
                return ParseBlockStatement();
            }
            if (Current.Kind == SyntaxKind.ForKeyword)
            {
                return ParseForStatement();
            }
            if (Current.Kind == SyntaxKind.LetKeyword || Current.Kind == SyntaxKind.VarKeyword)
            {
                return ParseVariableDeclaration();
            }
            if(Current.Kind==SyntaxKind.IfKeyWord)
            {
                return ParseIfStatement();
            }
            
            if(Current.Kind == SyntaxKind.WhileKeyword)
            {
                return ParseWhileStatement();
            }
            return ParseExpressionStatment();
        }

        private StatementSyntax ParseWhileStatement()
        {
            var WhileToken = Match(SyntaxKind.WhileKeyword);
            var condition = ParseExpression();
            var body=ParseStatement();
            return new WhileStatementSyntax(WhileToken, condition, body);
        }

        private StatementSyntax ParseForStatement()
        {
            var forToken = Match(SyntaxKind.ForKeyword);
            var identifierToken = Match(SyntaxKind.IdentifierToken);
            var equalsToken = Match(SyntaxKind.EqualsToken);
            var lowerBound = ParseExpression();
            var toToken = Match(SyntaxKind.ToKeyword);
            var upperBound = ParseExpression();
            var body = ParseStatement();
            return new ForStatementSyntax(forToken, identifierToken, equalsToken, lowerBound,toToken, upperBound, body);
        }

        private StatementSyntax ParseIfStatement()
        {
            var token = Match(SyntaxKind.IfKeyWord);
            var expression= ParseExpression();
            var statement = ParseStatement();
            var elseStatement = ParseElseCulse();
            return new IfStatementSyntax(token, expression, statement, elseStatement);
        }

        private ElseClauseSyntax ParseElseCulse()
        {
            if(Current.Kind!= SyntaxKind.ElseKeyword)
            {
                return null;
            }
            var token = NextToken();
            var statement = ParseStatement();
            return new ElseClauseSyntax(token, statement);
        }

        private StatementSyntax ParseVariableDeclaration()
        {
            var expected = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;
            var keywordToken = Match(expected);
            var identifierToken = Match(SyntaxKind.IdentifierToken);
            var equalsToken = Match(SyntaxKind.EqualsToken);
            var initializer = ParseExpression();
            if(Current.Kind == SyntaxKind.NewLineToken || Current.Kind == SyntaxKind.SemicolonToken)
            {
                NextToken();
            }
            return new VariableDeclarationSyntax(keywordToken, identifierToken,equalsToken, initializer);
        }

        private StatementSyntax ParseExpressionStatment()
        {
            var expression = ParseExpression();
            if(Current.Kind == SyntaxKind.NewLineToken||Current.Kind==SyntaxKind.SemicolonToken)
            {
                NextToken();
            }
            return new ExpressionStatementSyntax(expression);
        }

        private StatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();
            var openBrace = Match(SyntaxKind.OpenBraceToken);

            while (Current.Kind != SyntaxKind.CloseBraceToken
                && Current.Kind != SyntaxKind.EndOfFileToken)
            {
                // 跳过空行和分号
                if (Current.Kind == SyntaxKind.NewLineToken
                 || Current.Kind == SyntaxKind.SemicolonToken)
                {
                    NextToken();
                    continue;
                }

                // 解析一条语句；ParseStatement 内部要保证无论成功或失败都至少消费一个 token
                statements.Add(ParseStatement());
            }

            var closeBrace = Match(SyntaxKind.CloseBraceToken);
            return new BlockStatementSyntax(openBrace, statements.ToImmutable(), closeBrace);
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
