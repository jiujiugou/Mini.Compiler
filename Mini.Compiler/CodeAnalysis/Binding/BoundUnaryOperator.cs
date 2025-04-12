using Mini.Compiler.CodeAnalysis.Syntax;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(SyntaxKind syntaxKind,BoundUnaryOperatorKind kind,Type operandType)
            : this(syntaxKind, kind, operandType, operandType)
        {

        }
        public BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind operatorKind, Type operandType, Type resultType)
        {
            SyntaxKind = syntaxKind;
            OperatorKind = operatorKind;
            OperandType = operandType;
            ResultType = resultType;
        }
        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind OperatorKind { get; }
        public Type OperandType { get; }
        public Type ResultType { get; }
        private static BoundUnaryOperator[] operators =
        {
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
        };
        public static BoundUnaryOperator? Bind(SyntaxKind syntaxKind, Type operandType)
        {
            foreach (var op in operators)
            {
                if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
                {
                    return op;
                }
            }
            return null;
        }
    }
}
