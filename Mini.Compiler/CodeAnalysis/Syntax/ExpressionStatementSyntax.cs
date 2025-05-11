using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public sealed class ExpressionStatementSyntax: StatementSyntax
    {
        public ExpressionStatementSyntax(ExpressionSyntax expression) 
        {
            Expression = expression;
        }

        public ExpressionSyntax Expression { get; }
        public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
    }
}
