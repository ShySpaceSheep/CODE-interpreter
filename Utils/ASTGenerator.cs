using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CODE_interpreter.Utils
{
    public class ASTGenerator
    {
        static void Main(string[] args)
        {
            string outputDir = @"C:\Users\Lenovo\source\repos\CODE-interpreter-csharp\AST";
            DefineAst(outputDir, "Expression", new List<string>
            {
                "Assign   : Token Name, Expression Value",
                "Binary   : Expression Left, Token Operator, Expression Right",
                "Grouping : Expression Expr",
                "Literal  : Object Value",
                "Unary    : Token Operator, Expression Right",
                "Variable : Token Name"
            });
            DefineAst(outputDir, "Statement", new List<string>
            {
                "IfBlock    : List<Statement> Statements",
                "WhileBlock : List<Statement> Statements",
                "Expression : AST.Expression Expr",
                "If         : AST.Expression Condition, List<Statement> ThenBranch, List<Statement> ElseBranch",
                "Print      : AST.Expression Expr",
                "Var        : Token Name, AST.Expression Initializer",
                "While      : AST.Expression Condition, List<Statement> Body"
            });
        }
        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            string path = Path.Combine(outputDir, baseName + ".cs");
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();
                writer.WriteLine("using CODE_interpreter.Analyzers;");
                writer.WriteLine();
                writer.WriteLine("namespace CODE_interpreter.AST");
                writer.WriteLine("{");
                writer.WriteLine($"    public abstract class {baseName}");
                writer.WriteLine($"    {{");
                DefineVisitor(writer, baseName, types);
                foreach (string type in types)
                {
                    string[] parts = type.Split(new[] { ':' }, 2);
                    string className = parts[0].Trim();
                    string fields = parts[1].Trim();
                    DefineType(writer, baseName, className, fields);
                }
                writer.WriteLine();
                writer.WriteLine($"        public abstract R Accept<R>(IVisitor<R> visitor);");
                writer.WriteLine($"    }}");
                writer.WriteLine("}");
            }
        }
        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine($"        public interface IVisitor<R>");
            writer.WriteLine($"        {{");

            foreach (string type in types)
            {
                string typeName = type.Split(':')[0].Trim();
                writer.WriteLine($"            R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            writer.WriteLine($"        }}");
        }
        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine($"        public class {className} : {baseName}");
            writer.WriteLine($"        {{");

            // Constructor.
            writer.WriteLine($"            public {className}({fieldList})");
            writer.WriteLine($"            {{");

            // Store parameters in fields.
            string[] fields = fieldList.Split(new[] { ", " }, StringSplitOptions.None);
            foreach (string field in fields)
            {
                string[] parts = field.Split(' ');
                string name = parts[1];
                writer.WriteLine($"                this.{name} = {name};");
            }

            writer.WriteLine($"            }}");

            writer.WriteLine();
            writer.WriteLine($"            public override R Accept<R>(IVisitor<R> visitor)");
            writer.WriteLine($"            {{");
            writer.WriteLine($"                return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine($"            }}");

            // Fields.
            writer.WriteLine();
            foreach (string field in fields)
            {
                writer.WriteLine($"         public readonly {field};");
            }

            writer.WriteLine($"        }}");
        }
    }
}
