using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public class CompilationUnitSyntax:SyntaxNode
    {
        public SyntaxToken EndOfFileToken { get; }
        public StatementSyntax Statement { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;

        public CompilationUnitSyntax(SyntaxToken endOfFileToken,StatementSyntax statement)
        {
            EndOfFileToken = endOfFileToken;
            Statement = statement;
        }
        public override string ToString()
        {
            return $"{nameof(CompilationUnitSyntax)}: {EndOfFileToken}";
        }
    }
}
