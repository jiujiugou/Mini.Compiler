using Mini.Compiler.CodeAnalysis.Binding;
using Mini.Compiler.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Mini.Compiler.CodeAnalysis
{
    internal sealed class Compliation
    {
        public SyntaxTree SyntaxTree { get; }
        public Compliation Previous { get; }
        public Compliation(SyntaxTree syntaxTree) : this(null, syntaxTree)
        {
            SyntaxTree=syntaxTree;
        }
        public Compliation(Compliation previous,SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
            Previous = previous;
        }
        public BoundGlobalScope _globalScope;
        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(_globalScope, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }
                return _globalScope;
            }
        }
        public Compliation CompliationWith(SyntaxTree syntaxTree)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }
            return new Compliation(this,syntaxTree);
        }
        public EvaluationResult Evalutate(Dictionary<string, object> variables)
        {
            var syntaxTree = SyntaxTree;
            if (syntaxTree.Diagnostics.Any())
            {
                return new EvaluationResult(syntaxTree.Diagnostics, null!);
            }
            // 绑定
            // 1. 绑定表达式
            var BindGlobalScope = Binder.BindGlobalScope(null,syntaxTree.Root);
            var diagnostics = SyntaxTree.Diagnostics.Concat(BindGlobalScope.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null!);
            }

            var evaluator = new Evaluator(BindGlobalScope.BoundExpression, variables);
            var result = evaluator.Evaluate();

            return new EvaluationResult(ImmutableArray<Diagnostics>.Empty, result);

            return new EvaluationResult(Array.Empty<Diagnostics>(), result);
        }

    }

}
