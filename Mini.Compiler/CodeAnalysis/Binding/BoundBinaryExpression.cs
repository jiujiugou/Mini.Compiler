namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
        public BoundBinaryOperator Operator{ get; }
        public BoundExpression Left { get; }
        public BoundExpression Right { get; }
        public override Type Type => Left.Type;
    }
}
