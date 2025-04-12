namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundExpression operand, BoundUnaryOperator op)
        {
            Operand = operand;
            Operator = op;
        }

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public BoundUnaryOperator Operator{ get; }
        public BoundExpression Operand { get; }
        public override Type Type => Operand.Type;
    }
}
