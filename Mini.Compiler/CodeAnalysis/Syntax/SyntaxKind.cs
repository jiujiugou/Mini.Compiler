namespace Mini.Compiler.CodeAnalysis.Syntax
{
    // 定义一个枚举类型 SyntaxKind，用于表示不同的语法标记类型
    public enum SyntaxKind
    {
        // 标记类型
        // 数字标记
        NumberToken,
        // 空白标记
        WhiteSpaceToken,
        // 加号标记
        PlusToken,
        // 减号标记
        MinusToken,
        // 星号标记
        StarToken,
        // 斜杠标记
        SlashToken,
        // 左括号标记
        OpenParenthesisToken,
        // 右括号标记
        CloseParenthesisToken,
        // 错误标记
        BadToken,
        // 文件结束标记
        EndOfFileToken,
        // 标识符标记
        IdentifierToken,
        // & 标记
        AmpersandToken,
        // | 标记
        PipeToken,
        // ! 标记
        BangToken,
        // && 标记
        AmpersandEqualToken,
        // || 标记
        PipeEqualToken,
        // != 标记
        BangEqualToken,
        // == 标记
        EqualsEqualsToken,
        // = 标记
        EqualsToken,

        // 表达式类型
        // 二元表达式
        BinaryExpression,
        // 一元表达式
        UnaryExpression,
        // 字面量表达式
        LiteralExpression,
        // 括号表达式
        ParenthesizedExpression,

        // 关键字类型
        // true 关键字
        TrueKeyword,
        // false 关键字
        FalseKeyword,
        // null 关键字
        NullKeyword,
        // if 关键字
        IfKeyWord,
        // else 关键字
        ElseKeyword,
        // while 关键字
        WhileKeyword,
        // for 关键字
        ForKeyword,
        // return 关键字
        ReturnKeyword,
        NotEqualToken,
        NameExpression,
        AssginmentExpression,
        CompilationUnit,
        BlockStatement,
        ExpressionStatement,
        OpenBraceToken,
        CloseBraceToken,
        NewLineToken,
        SemicolonToken,
        VariableDeclaration,
        VarKeyword,
        LetKeyword,
        PipePipeToken,
        LessThanEqualsToken,
        LessThanToken,
        GreaterThanEqualsToken,
        GreaterThanToken,
        IfStatement,
        ElseClause,
        ForStatement,
        WhileStatement,
        ToKeyword
    }
}
