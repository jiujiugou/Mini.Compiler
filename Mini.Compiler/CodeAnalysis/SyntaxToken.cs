﻿namespace Mini.Compiler
{
    class SyntaxToken :SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int postion, string text, object value)
        {
            Kind = kind;
            Postion = postion;
            Text = text;
            Value = value;
        }
        public override SyntaxKind Kind { get; }
        public int Postion { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
