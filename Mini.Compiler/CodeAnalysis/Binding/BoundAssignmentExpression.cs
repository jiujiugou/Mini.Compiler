using Mini.Compiler.CodeAnalysis.Text;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public BoundExpression BoundExpression { get; }
        public VariableSymbol Variable { get; }

        public BoundAssignmentExpression(VariableSymbol variable,BoundExpression boundexpression)
        {
            BoundExpression = boundexpression;
            Variable = variable;
        }

        public override Type Type => BoundExpression.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignExpression;
    }
}
