﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(SyntaxToken openBraceToken,ImmutableArray<StatementSyntax> statements,SyntaxToken closeBraceToken)
        {
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceToken = closeBraceToken;
        }
        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public SyntaxToken OpenBraceToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }
    }
}
