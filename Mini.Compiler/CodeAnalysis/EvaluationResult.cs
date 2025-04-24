
﻿using System.Collections.Immutable;
using static Mini.Compiler.CodeAnalysis.Compliation;
﻿using static Mini.Compiler.CodeAnalysis.Compliation;

namespace Mini.Compiler.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IEnumerable<Diagnostics> diagnostics,object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }
        public IReadOnlyList<Diagnostics> Diagnostics { get; }
        public object Value { get; }
    }
}
