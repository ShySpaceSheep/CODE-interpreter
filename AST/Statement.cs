using System;
using System.Collections.Generic;

using CODEInterpreter.Analyzers;

namespace CODEInterpreter.AST
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
            R VisitScannerStatement(Scanner statement);
            R VisitVarStatement(Var statement);
            R VisitVarListStatement(VarList statement);
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
            public If(AST.Expression Condition, List<Statement> ThenBranch, List<If> AltBranches, List<Statement> ElseBranch)
            {
                this.Condition = Condition;
                this.ThenBranch = ThenBranch;
                this.ElseIfBranches = AltBranches;
                this.ElseBranch = ElseBranch;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitIfStatement(this);
            }

            public readonly AST.Expression Condition;
            public readonly List<Statement> ThenBranch;
            public readonly List<If> ElseIfBranches;
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
        public class Scanner : Statement
        {
            public Scanner(List<Var> Vars)
            {
                this.Vars = Vars;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitScannerStatement(this);
            }

            public readonly List<Var> Vars;
        }
        public class Var : Statement
        {
            public Var(Token.Type DataType, Token Name, AST.Expression Initializer)
            {
                this.DataType = DataType;
                this.Name = Name;
                this.Initializer = Initializer;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitVarStatement(this);
            }


            public readonly Token.Type DataType;
            public readonly Token Name;
            public readonly AST.Expression Initializer;
        }
        public class VarList : Statement
        {
            public VarList(List<Var> Declarations)
            {
                this.Declarations = Declarations;
            }

            public override R Accept<R>(IVisitor<R> visitor)
            {
                return visitor.VisitVarListStatement(this);
            }

         public readonly List<Var> Declarations;
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
