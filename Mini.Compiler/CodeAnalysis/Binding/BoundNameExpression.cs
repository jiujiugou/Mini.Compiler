using Mini.Compiler.CodeAnalysis.Text;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal class BoundNameExpression : BoundExpression
    {
        public VariableSymbol Variable;
        public BoundNameExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeKind Kind => BoundNodeKind.NameExpression;

        public override Type Type => Variable.Type;
    }
}
