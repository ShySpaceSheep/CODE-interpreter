using System.Text;

namespace CODEInterpreter.AST
{
    public class AstPrinter : Expression.IVisitor<string>
    {
        public string Print(Expression expr)
        {
            return expr.Accept(this);
        }
        public string VisitBinaryExpression(Expression.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitGroupingExpression(Expression.Grouping expr)
        {
            return Parenthesize("group", expr.Expr);
        }

        public string VisitLiteralExpression(Expression.Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitLogicalExpression(Expression.Logical expr)
        {
            return expr.ToString();
        }

        public string VisitUnaryExpression(Expression.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitVariableExpression(Expression.Variable var)
        {
            return var.ToString();
        }

        public string VisitAssignExpression(Expression.Assign var)
        {
            return var.ToString();
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
