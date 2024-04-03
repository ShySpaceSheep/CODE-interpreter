using CODE_interpreter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_interpreter.AST
{
    public class AstPrinter : Expression.IVisitor<string>
    {
        public string Print(Expression expr)
        {
            return expr.Accept(this);
        }
        public string VisitBinaryExpr(Expression.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpr(Expression.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitLiteralExpr(Expression.Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expression.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        private string Parenthesize(string name, params Expression[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expression expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
