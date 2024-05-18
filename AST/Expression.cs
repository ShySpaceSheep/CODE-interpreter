using System;

using CODEInterpreter.Analyzers;

namespace CODEInterpreter.AST
{
    public abstract class Expression
    {
        public interface IVisitor<R>
        {
            R VisitAssignExpression(Assign expression);
            R VisitBinaryExpression(Binary expression);
            R VisitGroupingExpression(Grouping expression);
            R VisitLiteralExpression(Literal expression);
            R VisitLogicalExpression(Logical expression);
            R VisitUnaryExpression(Unary expression);
            R VisitVariableExpression(Variable expression);
        }
        public class Assign : Expression
        {
            public Assign(Token Name, Expression Value)
            {
                this.Name = Name;
                this.Value = Value;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitAssignExpression(this);
            }

         public readonly Token Name;
         public readonly Expression Value;
        }
        public class Binary : Expression
        {
            public Binary(Expression Left, Token Operator, Expression Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitBinaryExpression(this);
            }

         public readonly Expression Left;
         public readonly Token Operator;
         public readonly Expression Right;
        }
        public class Grouping : Expression
        {
            public Grouping(Expression Expr)
            {
                this.Expr = Expr;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGroupingExpression(this);
            }

         public readonly Expression Expr;
        }
        public class Literal : Expression
        {
            public Literal(Object Value)
            {
                this.Value = Value;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLiteralExpression(this);
            }

         public readonly Object Value;
        }
        public class Logical : Expression
        {
            public Logical(Expression Left, Token Operator, Expression Right)
            {
                this.Left = Left;
                this.Operator = Operator;
                this.Right = Right;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLogicalExpression(this);
            }

         public readonly Expression Left;
         public readonly Token Operator;
         public readonly Expression Right;
        }
        public class Unary : Expression
        {
            public Unary(Token Operator, Expression Right)
            {
                this.Operator = Operator;
                this.Right = Right;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitUnaryExpression(this);
            }

         public readonly Token Operator;
         public readonly Expression Right;
        }
        public class Variable : Expression
        {
            public Variable(Token Name)
            {
                this.Name = Name;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitVariableExpression(this);
            }

         public readonly Token Name;
        }

        public abstract R Accept<R>(IVisitor<R> visitor);
    }
}
