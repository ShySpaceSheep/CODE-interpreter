using System;
using System.Collections.Generic;

using CODE_interpreter.Analyzers;
using CODE_interpreter.AST;


namespace CODE_interpreter
{
    public class Interpreter : Expression.IVisitor<Object>, Statement.IVisitor<object>
    {
        private readonly Environment Env = new Environment();

        public void Interpret(List<Statement> statements)
        {
            try
            {
                foreach (Statement statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (StdError.RuntimeError error)
            {
                StdError.ThrowRuntimeError(error);
            }
        }

        public Object VisitLiteralExpression(Expression.Literal expr)
        {
            return expr.Value;
        }

        public Object VisitGroupingExpression(Expression.Grouping expr)
        {
            return Evaluate(expr.Expr);
        }

        public Object VisitUnaryExpression(Expression.Unary expr)
        {
            Object right = Evaluate(expr.Right);
            switch (expr.Operator.TokenType) 
            {
                case Token.Type.NOT:
                    return IsTruthy(right);
                case Token.Type.SUB:
                    return -(double)right;
            }
            return null;
        }

        public Object VisitVariableExpression(Expression.Variable expr)
        {
            return Env.Get(expr.Name);
        }

        private void CheckNumberOperand(Token op, Object operand)
        {
            if (operand is Double) return;
            throw new StdError.RuntimeError(op, "TypeError: Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, Object left, Object right)
        {
            if ((left is int && right is int) || (left is double && right is double)) return;
            throw new StdError.RuntimeError(op, "TypeError: Cannot operate on numbers of different types.");
        }

        private void CheckZeroDivisor(Token op, Object right)
        {
            if (right is Double && !((Double) right == 0)) { return; }
            throw new StdError.RuntimeError(op, "ZeroDivisionError: Divisor must be non-zero.");
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

        private void Execute(Statement stmt)
        {
            stmt.Accept(this);
        }

        public Object VisitIfBlockStatement(Statement.IfBlock stmt)
        {
            ExecuteIfBlock(stmt.Statements);
            return null;
        }

        public object VisitWhileBlockStatement(Statement.WhileBlock stmt)
        {
            ExecuteWhileBlock(stmt.Statements);
            return null;
        }

        public Object VisitExpressionStatement(Statement.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        private void ExecuteIfBlock(List<Statement> stmts)
        {
            foreach (Statement s in stmts)
            {
                Execute(s);
            }
        }
        private void ExecuteWhileBlock(List<Statement> stmts)
        {
            foreach (Statement s in stmts)
            {
                Execute(s);
            }
        }

        public Object VisitIfStatement(Statement.If stmt)
        {
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                ExecuteIfBlock(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                ExecuteIfBlock(stmt.ElseBranch);
            }
            return null;
        }

        public Object VisitPrintStatement(Statement.Print stmt)
        {
            object value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public Object VisitVarStatement(Statement.Var stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
            }

            Env.DefineVar(stmt.Name, value);
            return null;
        }

        public Object VisitWhileStatement(Statement.While stmt)
        {
            while (IsTruthy(Evaluate(stmt.Condition)))
            {
                ExecuteWhileBlock(stmt.Body);
            }
            return null;
        }
        public Object VisitAssignExpression(Expression.Assign expr)
        {
            Object value = Evaluate(expr.Value);
            Env.Assign(expr.Name, value);
            return value;
        }

        public Object VisitBinaryExpression(Expression.Binary expr)
        {
            Object left = Evaluate(expr.Left);
            Object right = Evaluate(expr.Right);

            switch (expr.Operator.TokenType)
            {
                case Token.Type.CONCAT:
                    return left.ToString() + right.ToString();
                case Token.Type.ADD:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is int && right is int) { return (int)left + (int)right; }
                    if (left is double && right is double) { return (double)left + (double)right; }
                    return (double)left + (double)right;
                case Token.Type.SUB:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is int && right is int) { return (int)left - (int)right; }
                    if (left is double && right is double) { return (double)left - (double)right; }
                    return (double)left - (double)right;
                case Token.Type.DIV:
                    CheckNumberOperands(expr.Operator, left, right);
                    CheckZeroDivisor(expr.Operator, right);
                    if (left is int && right is int) { return (int)left / (int)right; }
                    if (left is double && right is double) { return (double)left / (double)right; }
                    return (double)left / (double)right;
                case Token.Type.MULT:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is int && right is int) { return (int)left * (int)right; }
                    if (left is double && right is double) { return (double)left * (double)right; }
                    return (double)left * (double)right;
                case Token.Type.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is int && right is int) { return (int)left > (int)right; }
                    if (left is double && right is double) { return (double)left > (double)right; }
                    return (double)left > (double)right;
                case Token.Type.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is int && right is int) { return (int)left >= (int)right; }
                    if (left is double && right is double) { return (double)left >= (double)right; }
                    return (double)left >= (double)right;
                case Token.Type.LESSER:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is int && right is int) { return (int)left < (int)right; }
                    if (left is double && right is double) { return (double)left < (double)right; }
                    return (double)left < (double)right;
                case Token.Type.LESSER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is int && right is int) { return (int)left <= (int)right; }
                    if (left is double && right is double) { return (double)left <= (double)right; }
                    return (double)left <= (double)right;
                case Token.Type.NOT_EQUAL: 
                    return !IsEqual(left, right);
                case Token.Type.EQUAL: 
                    return IsEqual(left, right);
            }

            // Unreachable.
            return null;
        }
    }
}
