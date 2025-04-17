using System.Collections;
using static Mini.Compiler.CodeAnalysis.Compliation;

namespace Mini.Compiler.CodeAnalysis.Syntax;

public class DiagnosticBag:IEnumerable<Diagnostics>
{
    private readonly List<Diagnostics> _diagnostics = new();
    public void Report(TextSpan span, string message)
    {
        var diagnostic = new Diagnostics(message, span);
        _diagnostics.Add(diagnostic);
    }

    public IEnumerator<Diagnostics> GetEnumerator() => _diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal void ReportInvalidNumber(TextSpan textSpan, string text, Type type)
    {
        var message = $"the number {text} isn't valid {type}";
        Report(textSpan,message);
    }

    internal void ReportBadCharacter(int position, char current)
    {
        var message = $"Bad character input {current} at position {position}";
        var textSpan = new TextSpan(position, 1);
        Report(textSpan, message);
    }

    internal void AddRange(DiagnosticBag diagnostics)
    {
        _diagnostics.AddRange(diagnostics._diagnostics);
    }

    internal void ReportUnexpectedToken(TextSpan span, SyntaxKind kind1, SyntaxKind kind2)
    {
        var message = $"Unexpected token {kind1}, expected {kind2}";
        Report(span, message);
    }

    internal void ReportUnDefinedUnaryOperator(TextSpan span, string text, Type type)
    {
        var message = $"Unary operator '{text}' is not defined for type {type}.";
        Report(span, message);
    }

    internal void ReportUnDefinedBinaryOperator(TextSpan span, string text, Type type1, Type type2)
    {
        var message = $"Binary operator '{text}' is not defined for types {type1} and {type2}.";
        Report(span, message);
    }

    internal void ReportUndefinedName(object span, string name)
    {
        var message = $"Undefined name '{name}'.";
        Report((TextSpan)span, message);
    }

    internal void ReportNotAssginment(TextSpan span, Type type1, Type type2)
    {
        var message = $"Cannot assign {type1} to {type2}.";
        Report(span, message);
    }
}
