using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(SyntaxToken keyword, SyntaxToken identifier, SyntaxToken equals, ExpressionSyntax initializer)
        {
            Keyword = keyword;
            Identifier = identifier;
            Equals = equals;
            Initializer = initializer;
        }
        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken Equals { get; }
        public ExpressionSyntax Initializer { get; }
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
    }
}
