using System.Collections.Immutable;
using System.Linq.Expressions;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal abstract class BoundStatement : BoundNode
    {

    }
    internal class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }
        public ImmutableArray<BoundStatement> Statements { get; }
        public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
    }
}
