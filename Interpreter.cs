using CODE_interpreter.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_interpreter
{
    public class Interpreter : Expression.IVisitor<Object>
    {
        public class RuntimeError : Exception
        {
            public Parser.Token Token { get; }
            public RuntimeError(Parser.Token token, string message) : base(message)
            {
                Token = token;
            }
        }

        public void Interpret(Expression expression)
        {
            try
            {
                Object value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                CODE.RuntimeError(error);
            }
        }
        public Object VisitLiteralExpr(Expression.Literal expr)
        {
            return expr.Value;
        }

        public Object VisitGroupingExpr(Expression.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public Object VisitUnaryExpr(Expression.Unary expr)
        {
            Object right = Evaluate(expr.Right);
            switch (expr.Operator.CurrType) 
            {
                case Parser.Token.Type.NOT:
                    return IsTruthy(right);
                case Parser.Token.Type.SUB:
                    return -(double)right;
            }
            return null;
        }

        private void CheckNumberOperand(Parser.Token op, Object operand)
        {
            if (operand is Double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Parser.Token op, Object left, Object right)
        {
            if (left is Double && right is Double) return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }

        private void CheckZeroDivisor(Parser.Token op, Object right)
        {
            if (right is Double && !((Double) right == 0)) { return; }
            throw new RuntimeError(op, "Division by zero.");
        }
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(Object a, Object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            return obj.ToString();
        }
        private Object Evaluate(Expression expr)
        {
            return expr.Accept(this);
        }
        
        public Object VisitBinaryExpr(Expression.Binary expr)
        {
            Object left = Evaluate(expr.Left);
            Object right = Evaluate(expr.Right);

            switch (expr.Operator.CurrType)
            {
                case Parser.Token.Type.CONCAT:
                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }
                    throw new RuntimeError(expr.Operator, "Operands must be two strings.");
                case Parser.Token.Type.ADD:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left + (double)right;
                case Parser.Token.Type.SUB:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case Parser.Token.Type.DIV:
                    CheckNumberOperands(expr.Operator, left, right);
                    CheckZeroDivisor(expr.Operator, right);
                    return (double)left / (double)right;
                case Parser.Token.Type.MULT:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case Parser.Token.Type.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case Parser.Token.Type.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case Parser.Token.Type.LESSER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case Parser.Token.Type.LESSER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                case Parser.Token.Type.NOT_EQUAL: 
                    return !IsEqual(left, right);
                case Parser.Token.Type.EQUAL: 
                    return IsEqual(left, right);
            }

            // Unreachable.
            return null;
        }
    }
}
