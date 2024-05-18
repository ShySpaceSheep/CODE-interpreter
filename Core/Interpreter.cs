using System;
using System.Collections.Generic;

using CODEInterpreter.Analyzers;
using CODEInterpreter.AST;
using CODEInterpreter.Errors;
using CODEInterpreter.Strings;

namespace CODEInterpreter
{
    public class Interpreter : Expression.IVisitor<Object>, Statement.IVisitor<object>
    {
        private readonly Environment _env = new();

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

        public Object VisitLogicalExpression(Expression.Logical expr)
        {
            Object left = Evaluate(expr.Left);

            if (expr.Operator.TokenType == Token.Type.OR) {
                if (IsTruthy(left)) return left;
            } else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.Right);
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
                    return !IsTruthy(right);
                case Token.Type.SUB:
                    if (right is int) { return -(int) right; }
                    if (right is double) { return -(double) right; }
                    throw new StdError.RuntimeError(expr.Operator, "Operation error: Cannot operate on non-numerical type.");
            }
            return null;
        }

        public Object VisitVariableExpression(Expression.Variable expr)
        {
            return _env.Get(expr.Name);
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
            bool allChoicesUnsatisfactory = false;
            if (IsTruthy(Evaluate(stmt.Condition)))
            {
                ExecuteIfBlock(stmt.ThenBranch);
                return null;
            }
            else if (stmt.ElseIfBranches != null)
            {
                foreach (Statement.If elifBranches in stmt.ElseIfBranches)
                {
                    if (IsTruthy(Evaluate(elifBranches.Condition)))
                    {
                        ExecuteIfBlock(elifBranches.ThenBranch);
                        return null;
                    }
                }
            }

            allChoicesUnsatisfactory = true;
            if (stmt.ElseBranch != null && allChoicesUnsatisfactory)
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

        public Object VisitScannerStatement(Statement.Scanner sc)
        {
            System.IO.StreamReader reader = new(Console.OpenStandardInput());
            foreach (Statement.Var var in sc.Vars)
            {
                Console.Write(var.Name.Lexeme + ": ");
                string value = reader.ReadLine();

                object parsedValue = null;
                if (int.TryParse(value, out int parsedInt))
                {
                    parsedValue = parsedInt;
                }
                else if (double.TryParse(value, out double parsedDob))
                {
                    parsedValue = parsedDob;
                }
                else if (char.TryParse(value, out char parsedChar))
                {
                    parsedValue = parsedChar;
                }
                else if (bool.TryParse(value, out bool parsedBool))
                {
                    parsedValue = parsedBool;
                }

                _env.Assign(var.Name, parsedValue);
            }
            return null;
        }

        public Object VisitVarStatement(Statement.Var stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = Evaluate(stmt.Initializer);
                if (stmt.DataType == Token.Type.VAR_INT)
                {
                    if (value is not int) 
                    { 
                        throw new StdError.RuntimeError(stmt.Name, TypeError.IncompatibleType(stmt.Name.Lexeme, stmt.DataType.ToString()));
                    }
                } 
                else if (stmt.DataType == Token.Type.VAR_FLOAT)
                {
                    if (value is not double)
                    {
                        throw new StdError.RuntimeError(stmt.Name, TypeError.IncompatibleType(stmt.Name.Lexeme, stmt.DataType.ToString()));
                    }
                }
                else if (stmt.DataType == Token.Type.VAR_CHAR)
                {
                    if (value is not char)
                    {
                        throw new StdError.RuntimeError(stmt.Name, TypeError.IncompatibleType(stmt.Name.Lexeme, stmt.DataType.ToString()));
                    }
                }
                else if (stmt.DataType == Token.Type.VAR_BOOL)
                {
                    if (value is not bool)
                    {
                        throw new StdError.RuntimeError(stmt.Name, TypeError.IncompatibleType(stmt.Name.Lexeme, stmt.DataType.ToString()));
                    }
                }
            }

            _env.DefineVar(stmt.DataType, stmt.Name, value);
            return null;
        }

        public Object VisitVarListStatement(Statement.VarList declarations)
        {
            foreach (Statement.Var stmt in declarations.Declarations)
            {
                VisitVarStatement(stmt);
            }
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
            _env.Assign(expr.Name, value);
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
                    if (left is int && right is int) 
                    {
                        return (int)left + (int)right; 
                    }
                    if (left is double && right is double) { return (double)left + (double)right; }
                    //return (double)left + (double)right;
                    break;
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
                case Token.Type.MOD:
                    CheckNumberOperands(expr.Operator, left, right);
                    if (left is not int && right is not int) { throw new StdError.RuntimeError(expr.Operator, "OperationError: Operands must be int"); }
                    return (int) left % (int) right;
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
            return null;
        }
    }
}
