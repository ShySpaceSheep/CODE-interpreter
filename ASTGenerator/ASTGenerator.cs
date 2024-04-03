using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_interpreter.ASTGenerator
{
    public class ASTGenerator
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(64);
            }
            string outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string>
            {
                "Binary   : Expr left, Token operator, Expr right",
                "Grouping : Expr expression",
                "Literal : Object value",
                "Unary    : Token operator, Expr right"
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
                writer.WriteLine("namespace CODE-interpreter.AST");
                writer.WriteLine("{");
                writer.WriteLine($"    public abstract class {baseName}");
                writer.WriteLine("    {");
                DefineVisitor(writer, baseName, types);
                foreach (string type in types)
                {
                    string[] parts = type.Split(new[] { ':' }, 2);
                    string className = parts[0].Trim();
                    string fields = parts[1].Trim();
                    DefineType(writer, baseName, className, fields);
                }
                writer.WriteLine();
                writer.WriteLine(" R Accept<R>(Visitor<R> visitor);");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
        }
        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine(" public interface Visitor<R>");
            writer.WriteLine(" {");

            foreach (string type in types)
            {
                string typeName = type.Split(':')[0].Trim();
                writer.WriteLine($"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            writer.WriteLine(" }");
        }
        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine($" public static class {className} : {baseName}");
            writer.WriteLine(" {");

            // Constructor.
            writer.WriteLine($"    public {className}({fieldList})");
            writer.WriteLine("    {");

            // Store parameters in fields.
            string[] fields = fieldList.Split(new[] { ", " }, StringSplitOptions.None);
            foreach (string field in fields)
            {
                string[] parts = field.Split(' ');
                string name = parts[1];
                writer.WriteLine($"      this.{name} = {name};");
            }

            writer.WriteLine("    }");

            writer.WriteLine();
            writer.WriteLine("    public override R Accept<R>(Visitor<R> visitor)");
            writer.WriteLine("    {");
            writer.WriteLine($"      return visitor.Visit{className}{baseName}(this);");
            writer.WriteLine("    }");

            // Fields.
            writer.WriteLine();
            foreach (string field in fields)
            {
                writer.WriteLine($"    public readonly {field};");
            }

            writer.WriteLine(" }");
        }
    }
}
