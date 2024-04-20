using System.Collections.Generic;

using CODE_interpreter.Analyzers;

namespace CODE_interpreter.AST
{
    public abstract class Statement
    {
        public interface IVisitor<R>
        {
            R VisitIfBlockStatement(IfBlock statement);
            R VisitWhileBlockStatement(WhileBlock statement);
            R VisitExpressionStatement(Expression statement);
            R VisitIfStatement(If statement);
            R VisitPrintStatement(Print statement);
            R VisitVarStatement(Var statement);
            R VisitWhileStatement(While statement);
        }
        public class IfBlock : Statement
        {
            public IfBlock(List<Statement> Statements)
            {
                this.Statements = Statements;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitIfBlockStatement(this);
            }

         public readonly List<Statement> Statements;
        }
        public class WhileBlock : Statement
        {
            public WhileBlock(List<Statement> Statements)
            {
                this.Statements = Statements;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitWhileBlockStatement(this);
            }

         public readonly List<Statement> Statements;
        }
        public class Expression : Statement
        {
            public Expression(AST.Expression Expr)
            {
                this.Expr = Expr;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitExpressionStatement(this);
            }

         public readonly AST.Expression Expr;
        }
        public class If : Statement
        {
            public If(AST.Expression Condition, List<Statement> ThenBranch, List<Statement> ElseBranch)
            {
                this.Condition = Condition;
                this.ThenBranch = ThenBranch;
                this.ElseBranch = ElseBranch;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitIfStatement(this);
            }

         public readonly AST.Expression Condition;
         public readonly List<Statement> ThenBranch;
         public readonly List<Statement> ElseBranch;
        }
        public class Print : Statement
        {
            public Print(AST.Expression Expr)
            {
                this.Expr = Expr;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitPrintStatement(this);
            }

         public readonly AST.Expression Expr;
        }
        public class Var : Statement
        {
            public Var(Token Name, AST.Expression Initializer)
            {
                this.Name = Name;
                this.Initializer = Initializer;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitVarStatement(this);
            }

         public readonly Token Name;
         public readonly AST.Expression Initializer;
        }
        public class While : Statement
        {
            public While(AST.Expression Condition, List<Statement> Body)
            {
                this.Condition = Condition;
                this.Body = Body;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitWhileStatement(this);
            }

         public readonly AST.Expression Condition;
         public readonly List<Statement> Body;
        }

        public abstract R Accept<R>(IVisitor<R> visitor);
    }
}
