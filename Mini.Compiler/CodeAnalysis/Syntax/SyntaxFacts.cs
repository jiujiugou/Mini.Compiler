
namespace Mini.Compiler.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetBinaryOperatorPrecedence(SyntaxKind kind)
    => kind switch
    {
        // 乘除        -> 5
        SyntaxKind.StarToken or SyntaxKind.SlashToken => 5,

        // 加减        -> 4
        SyntaxKind.PlusToken or SyntaxKind.MinusToken => 4,

        // 关系运算符  -> 3
        SyntaxKind.LessThanToken or SyntaxKind.LessThanEqualsToken
      or SyntaxKind.GreaterThanToken or SyntaxKind.GreaterThanEqualsToken => 3,

        // 相等运算符  -> 2
        SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualToken => 2,

        // 按位与      -> 1
        SyntaxKind.AmpersandToken => 1,

        // 按位或      -> 0
        SyntaxKind.PipeToken => 0,

        _ => 0
    };
        public static int GetUnaryOperatorPrecedence(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.BangToken => 4, 
                SyntaxKind.PlusToken => 4, 
                SyntaxKind.MinusToken => 4,
                _ => 0,
            };
        }
        internal static SyntaxKind GetKeywordKind(string text)
        {
            switch (text)
            {
                case "true": return SyntaxKind.TrueKeyword;
                case "false": return SyntaxKind.FalseKeyword;
                case "null": return SyntaxKind.NullKeyword;
                case "var": return SyntaxKind.VarKeyword;
                case "let": return SyntaxKind.LetKeyword;
                case "if": return SyntaxKind.IfKeyWord;
                case "else": return SyntaxKind.ElseKeyword;
                case "while": return SyntaxKind.WhileKeyword;
                case "for": return SyntaxKind.ForKeyword;
                case "to": return SyntaxKind.ToKeyword;
                case "return": return SyntaxKind.ReturnKeyword;
                default: return SyntaxKind.IdentifierToken;
            }
        }
        internal static string GetText(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken: return "+";
                case SyntaxKind.MinusToken: return "-";
                case SyntaxKind.StarToken: return "*";
                case SyntaxKind.SlashToken: return "/";
                case SyntaxKind.OpenParenthesisToken: return "(";
                case SyntaxKind.CloseParenthesisToken: return ")";
                case SyntaxKind.BadToken: return "!";
                case SyntaxKind.AmpersandToken: return "&";
                case SyntaxKind.OpenBraceToken: return "{";
                case SyntaxKind.CloseBraceToken: return "}";
                case SyntaxKind.AmpersandEqualToken: return "&&";
                case SyntaxKind.PipePipeToken: return "||";
                case SyntaxKind.BangEqualToken: return "!=";
                case SyntaxKind.EqualsEqualsToken: return "==";
                case SyntaxKind.EqualsToken: return "=";
                case SyntaxKind.SemicolonToken:return ";";
                case SyntaxKind.LessThanToken: return "<";
                case SyntaxKind.LessThanEqualsToken: return "<=";
                case SyntaxKind.GreaterThanToken: return ">";
                case SyntaxKind.GreaterThanEqualsToken: return ">=";
                case SyntaxKind.IfKeyWord: return "if";
                case SyntaxKind.ElseKeyword: return "else";
                case SyntaxKind.VarKeyword: return "var";
                case SyntaxKind.LetKeyword: return "let";
                case SyntaxKind.TrueKeyword: return "true";
                case SyntaxKind.FalseKeyword: return "false";
                case SyntaxKind.EndOfFileToken:return "\0";
                case SyntaxKind.NewLineToken:return "\n";
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
    }
}