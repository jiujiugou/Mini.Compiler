using Mini.Compiler.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> _variable = new Dictionary<string, VariableSymbol>();
        public BoundScope Parent { get; }
        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }
        public bool DeclareVariable(string name, VariableSymbol variable)
        {
            if(_variable.ContainsKey(name))
                return false;
            _variable.Add(name, variable);
            return true;
        }
        public bool TryDeclare(VariableSymbol variable)
        {
            if (_variable.ContainsKey(variable.Name))
                return false;
            _variable.Add(variable.Name, variable);
            return true;
        }
        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
            if (_variable.TryGetValue(name, out variable))
                return true;
            if (Parent != null)
                return Parent.TryLookupVariable(name, out variable);
            variable = null;
            return false;
        }
        public ImmutableArray<VariableSymbol> GetVariableSymbols()
        {
            return _variable.Values.ToImmutableArray();
        }
        public ImmutableArray<VariableSymbol> GetDeclearedVariables()
        {
            return _variable.Values.ToImmutableArray();
        }
    }
}
