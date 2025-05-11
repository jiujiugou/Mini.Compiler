using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    internal class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax(SyntaxToken ifKeyword,ExpressionSyntax condition,StatementSyntax thenStatemrnt,
                                ElseClauseSyntax elseClause)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenStatemrnt = thenStatemrnt;
            ElseClause = elseClause;
        }
        public override SyntaxKind Kind => SyntaxKind.IfStatement;

        public SyntaxToken IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax ThenStatemrnt { get; }
        public ElseClauseSyntax ElseClause { get; }
    }
    public class ElseClauseSyntax : SyntaxNode
    {
        public ElseClauseSyntax(SyntaxToken elseKeyword, StatementSyntax statement)
        {
            ElseKeyword = elseKeyword;
            Statement = statement;
        }
        public SyntaxToken ElseKeyword { get; }
        public StatementSyntax Statement { get; }
        public override SyntaxKind Kind => SyntaxKind.ElseClause;
    }
}
