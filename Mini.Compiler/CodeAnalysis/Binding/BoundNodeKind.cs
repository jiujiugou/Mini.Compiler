namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        ErrorExpression,
        NameExpression,
        AssignExpression,
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        ForStatement,
        WhileStatement
    }
}
