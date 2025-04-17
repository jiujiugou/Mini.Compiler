using System.Reflection;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span
        {
            get
            {
                var children = GetChildren();
                if (!children.Any())
                    return new TextSpan(0, 0);
                var start = children.First().Span.Start;
                var end = children.Last().Span.End;
                return new TextSpan(start, end - start);
            }
        }

        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this)!;
                    if (child != null)
                    {
                        yield return child;
                    }
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this)!;
                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}
