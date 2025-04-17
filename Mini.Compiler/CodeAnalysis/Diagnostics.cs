namespace Mini.Compiler.CodeAnalysis
{
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
