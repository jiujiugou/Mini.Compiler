namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal class BoundIfStatement : BoundStatement
    {
        public BoundExpression expression;
        public BoundStatement thenStatement;
        public BoundStatement? elseStatement;

        public BoundIfStatement(BoundExpression expression, BoundStatement thenStatement, BoundStatement? elseStatement)
        {
            this.expression = expression;
            this.thenStatement = thenStatement;
            this.elseStatement = elseStatement;
        }

        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    }
}