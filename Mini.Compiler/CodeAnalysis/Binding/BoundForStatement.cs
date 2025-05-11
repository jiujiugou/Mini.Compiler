using Mini.Compiler.CodeAnalysis.Text;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal class BoundForStatement : BoundStatement
    {
        public VariableSymbol variable;
        public BoundExpression lowerBound;
        public BoundExpression upperBound;
        public BoundStatement body;

        public BoundForStatement(VariableSymbol variable, BoundExpression lowerBound, BoundExpression upperBound, BoundStatement body)
        {
            this.variable = variable;
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
            this.body = body;
        }

        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
    }
}
