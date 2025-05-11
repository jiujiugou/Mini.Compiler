namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }
        
        public override BoundNodeKind Kind => BoundNodeKind.ExpressionStatement;

        public BoundExpression Expression { get; }
    }
}
