using Mini.Compiler.CodeAnalysis.Syntax;

namespace Mini.Compiler.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type operandType)
            : this(syntaxKind, kind, operandType, operandType, operandType)
        {

        }
        private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, Type operandType,Type resultType)
            : this(syntaxKind, kind, operandType, operandType, resultType)
        {

        }
        public BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind operatorKind, Type leftType, Type rightType, Type resultType)
        {
            SyntaxKind = syntaxKind;
            OperatorKind = operatorKind;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }
        public SyntaxKind SyntaxKind { get; }
        public BoundBinaryOperatorKind OperatorKind { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type ResultType { get; }
        private static BoundBinaryOperator[] _operators = {
            new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition,typeof(int)),
            new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction,typeof(int)),
            new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication,typeof(int)),
            new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, typeof(int)),
            new BoundBinaryOperator(SyntaxKind.AmpersandToken, BoundBinaryOperatorKind.LogicalAnd,typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.LogicalOr,typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals,typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.BangEqualToken, BoundBinaryOperatorKind.NotEquals,typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.LessThanToken, BoundBinaryOperatorKind.Less,typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.LessThanEqualsToken, BoundBinaryOperatorKind.LessOrEqualTo,typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.GreaterThanToken, BoundBinaryOperatorKind.Greater,typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxKind.GreaterThanEqualsToken, BoundBinaryOperatorKind.GreaterOrEqualTo,typeof(int), typeof(bool)),
            //new BoundBinaryOperator(SyntaxKind.AmpersandEqualToken, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
        };
        public static BoundBinaryOperator? Bind(SyntaxKind syntaxKind, Type leftType, Type rightType)
        {
            foreach (var op in _operators)
            {
                if (op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightType == rightType)
                {
                    return op;
                }
            }
            return null;
        }
    }
}
