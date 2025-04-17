namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal class BoundNameExpression : BoundExpression
    {
        public string Name { get; }
        public override Type Type { get; }
        public object? Value { get; }
        public BoundNameExpression(string name, Type type, object? value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public override BoundNodeKind Kind => BoundNodeKind.NameExpression;
    }
}
