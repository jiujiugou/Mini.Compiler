using Mini.Compiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        internal BoundGlobalScope(BoundGlobalScope boundGlobal, ImmutableArray<Diagnostics> diagnostics, ImmutableArray<VariableSymbol> variableSymbols, BoundStatement boundExpression)
        {
            BoundGlobal = boundGlobal;
            Diagnostics = diagnostics;
            VariableSymbols = variableSymbols;
            BoundExpression = boundExpression;
        }

        public BoundGlobalScope BoundGlobal { get; }
        public ImmutableArray<Diagnostics> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> VariableSymbols { get; }
        public BoundStatement BoundExpression { get; }
    }
}
