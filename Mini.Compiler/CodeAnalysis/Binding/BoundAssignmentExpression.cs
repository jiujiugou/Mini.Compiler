namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public BoundExpression BoundExpression { get; }
        public string Name { get; }

        public BoundAssignmentExpression(string name,BoundExpression boundexpression)
        {
            BoundExpression = boundexpression;
            Name = name;
        }

        public override Type Type => BoundExpression.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignExpression;
    }
}
