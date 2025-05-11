using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    internal class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(SyntaxToken fortoken, SyntaxToken identifier,SyntaxToken equalToken,ExpressionSyntax lowerBound,SyntaxToken totoken,ExpressionSyntax upperBound,StatementSyntax body) 
        {
            ForToken = fortoken;
            Identifier = identifier;
            EqualToken = equalToken;
            LowerBound = lowerBound;
            ToToken = totoken;
            UpperBound = upperBound;
            Body = body;
        }
        public override SyntaxKind Kind => SyntaxKind.ForStatement;

        public SyntaxToken ForToken { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualToken { get; }
        public ExpressionSyntax LowerBound { get; }
        public SyntaxToken ToToken { get; }
        public ExpressionSyntax UpperBound { get; }
        public StatementSyntax Body { get; }
    }
}
