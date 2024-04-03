using CODE_interpreter.Parser;
using System;
using System.Collections.Generic;

namespace CODE_interpreter.AST
{
    public abstract class Expression
    {
        public interface IVisitor<R>
        {
            R VisitBinaryExpr(Binary expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitLiteralExpr(Literal expr);
            R VisitUnaryExpr(Unary expr);
        }

        public class Binary : Expression
        {
            public Binary(Expression left, Token op, Expression right)
            {
                this.Left = left;
                this.Operator = op;
                this.Right = right;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }

            public readonly Expression Left;
            public readonly Token Operator;
            public readonly Expression Right;
        }

        public class Grouping : Expression
        {
            public Grouping(Expression expression)
            {
                this.Expression = expression;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }

            public readonly Expression Expression;
        }

        public class Literal : Expression
        {
            public Literal(object value)
            {
                this.Value = value;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }

            public readonly object Value;
        }

        public class Unary : Expression
        {
            public Unary(Token op, Expression right)
            {
                this.Operator = op;
                this.Right = right;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }

            public readonly Token Operator;
            public readonly Expression Right;
        }

        public abstract R Accept<R>(IVisitor<R> visitor);
    }
}
