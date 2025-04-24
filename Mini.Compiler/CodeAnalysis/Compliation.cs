using Mini.Compiler.CodeAnalysis.Binding;
using Mini.Compiler.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace Mini.Compiler.CodeAnalysis
{
    public sealed class Compliation
    {
        public Compliation(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }
        public SyntaxTree SyntaxTree { get; }
        public EvaluationResult Evalutate(Dictionary<string, object> variables)
        {
            var syntaxTree = SyntaxTree;
            if (syntaxTree.Diagnostics.Any())
            {
                return new EvaluationResult(syntaxTree.Diagnostics, null!);
            }
            // 绑定
            // 1. 绑定表达式
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(SyntaxTree.Root);
            var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToImmutableArray();
            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null!);
            }

            var evaluator = new Evaluator(boundExpression,variables);
            var result = evaluator.Evaluate();

            return new EvaluationResult(ImmutableArray<Diagnostics>.Empty, result);

            return new EvaluationResult(Array.Empty<Diagnostics>(), result);
        }
    }
    public struct TextSpan
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
        public int Start { get; }
        public int Length { get; }

        public int End => Start + Length;
    }
    public class Diagnostics
    {
        public Diagnostics(string message, TextSpan span)
        {
            Message = message;
            Span = span;
        }
        public string Message { get; }
        public TextSpan Span { get; }
        public override string ToString()
        {
            return Message;
        }
    }
}
