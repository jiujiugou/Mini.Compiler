using Mini.Compiler.CodeAnalysis.Binding;
using System;

namespace Mini.Compiler.CodeAnalysis
{

    class Evaluator
    {
        private object _lastvalue=null;
        private readonly BoundStatement _root;
        private Dictionary<string, object> _variables;
        public Evaluator(BoundStatement root, Dictionary<string, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(_root);
            return _lastvalue;
        }
        private void EvaluateStatement(BoundStatement statement)
        {
            switch (statement.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement)statement);
                    break;
                case BoundNodeKind.WhileStatement:
                    EvaluateWhileStatement((BoundWhileStatement)statement);
                    break;
                case BoundNodeKind.IfStatement:
                    EvaluateIfStatement((BoundIfStatement)statement);
                    break;
                case BoundNodeKind.ForStatement:
                    EvaluateForStatement((BoundForStatement)statement);
                    break;
                case BoundNodeKind.VariableDeclaration:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)statement);
                    break;
                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    break;
                default:
                    throw new Exception($"Unexpected node kind: {statement.Kind}");
            }
        }

        private void EvaluateWhileStatement(BoundWhileStatement statement)
        {
            while((bool)EvaluateExpression(statement.Condition))
            {
                EvaluateStatement(statement.Body);
            }
        }

        private void EvaluateForStatement(BoundForStatement stmt)
        {
            var lo = (int)EvaluateExpression(stmt.lowerBound);
            var hi = (int)EvaluateExpression(stmt.upperBound);

            // （不换 _variables 字典，只复用它，让对 a 的写入也生效）
            for (int v = lo; v <= hi; v++)
            {
                _variables[stmt.variable.Name] = v;
                EvaluateStatement(stmt.body);
            }

            // 循环结束后可选：移除循环变量（如果你想让它块级消失）
            _variables.Remove(stmt.variable.Name);
        }



        private void EvaluateIfStatement(BoundIfStatement statement)
        {
            var condition = (bool)EvaluateExpression(statement.expression);
            if (condition)
            {
                EvaluateStatement(statement.thenStatement);
            }
            else if (statement.elseStatement != null)
            {
                EvaluateStatement(statement.elseStatement);
            }
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration statement)
        {
            var value=EvaluateExpression(statement.Initializer);
            _variables[statement.Variable.Name] = value;
            _lastvalue = value;
        }

        private void EvaluateBlockStatement(BoundBlockStatement statement)
        {
            foreach (var s in statement.Statements)
            {
                EvaluateStatement(s);
            }
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement)
        {
            _lastvalue = EvaluateExpression(statement.Expression);
        }

        private object EvaluateExpression(BoundExpression root)
        {
            switch (root)
            {
                case BoundLiteralExpression literal:
                    return literal.Value;

                case BoundNameExpression name:
                    return _variables[name.Variable.Name];
                case BoundAssignmentExpression assignment:
                    var value = EvaluateExpression(assignment.BoundExpression);
                    _variables[assignment.Variable.Name] = value;
                    return value;

                case BoundUnaryExpression unary:
                    var operand = EvaluateExpression(unary.Operand);
                    switch (unary.Operator.OperatorKind)
                    {
                        case BoundUnaryOperatorKind.Identity:
                            return operand;
                        case BoundUnaryOperatorKind.Negation:
                            return -(int)operand;
                        case BoundUnaryOperatorKind.LogicalNegation:
                            return !(bool)operand;
                        default:
                            throw new Exception($"Unexpected unary operator {unary.Operator.OperatorKind}");
                    }

                case BoundBinaryExpression binary:
                    var left = EvaluateExpression(binary.Left);
                    var right = EvaluateExpression(binary.Right);
                    switch (binary.Operator.OperatorKind)
                    {
                        case BoundBinaryOperatorKind.Addition:
                            return (int)left + (int)right;
                        case BoundBinaryOperatorKind.Subtraction:
                            return (int)left - (int)right;
                        case BoundBinaryOperatorKind.Multiplication:
                            return (int)left * (int)right;
                        case BoundBinaryOperatorKind.Division:
                            if ((int)right == 0)
                                throw new DivideByZeroException("Cannot divide by zero.");
                            return (int)left / (int)right;
                        case BoundBinaryOperatorKind.LogicalAnd:
                            return (bool)left && (bool)right;
                        case BoundBinaryOperatorKind.LogicalOr:
                            return (bool)left || (bool)right;
                        case BoundBinaryOperatorKind.Equals:
                            return Equals(left, right);
                        case BoundBinaryOperatorKind.NotEquals:
                            return !Equals(left, right);
                        case BoundBinaryOperatorKind.Less:
                            return (int)left < (int)right;
                        case BoundBinaryOperatorKind.LessOrEqualTo:
                            return (int)left <= (int)right;
                        case BoundBinaryOperatorKind.Greater:
                            return (int)left > (int)right;
                        case BoundBinaryOperatorKind.GreaterOrEqualTo:
                            return (int)left >= (int)right;
                        default:
                            throw new Exception($"Unexpected binary operator {binary.Operator.OperatorKind}");
                    }

                default:
                    throw new Exception($"Unexpected node kind: {root.Kind}");
            }
        }
    }
}
//{
//    a=10
//    a=100
//    a*a
//}
