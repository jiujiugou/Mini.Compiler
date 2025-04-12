
namespace Mini.Compiler.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetBinaryOperatorPrecedence(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.StarToken or SyntaxKind.SlashToken => 4,
                SyntaxKind.PlusToken or SyntaxKind.MinusToken => 3,
                SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualToken => 2,
                SyntaxKind.AmpersandToken => 1,
                SyntaxKind.PipeToken => 0,
                _ => 0,
            };

        }
        public static int GetUnaryOperatorPrecedence(SyntaxKind kind)
        {
            return kind switch
            {
                SyntaxKind.BangToken => 3, // 逻辑非优先级最高
                SyntaxKind.PlusToken => 3, // 正号优先级最高
                SyntaxKind.MinusToken => 3, // 负号优先级最高
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
                case "if": return SyntaxKind.IfKeyword;
                case "else": return SyntaxKind.ElseKeyword;
                case "while": return SyntaxKind.WhileKeyword;
                case "for": return SyntaxKind.ForKeyword;
                case "return": return SyntaxKind.ReturnKeyword;
                default: return SyntaxKind.IdentifierToken;
            }
        }
    }
}