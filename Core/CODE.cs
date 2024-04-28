using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using CODE_interpreter.CODEStrings;
using CODE_interpreter.Analyzers;
using CODE_interpreter.AST;

namespace CODE_interpreter
{
    public class CODE
    {
        [Flags]
        public enum ExitType: int
        {
            EXIT_SUCCESS = 0,
            EXIT_ERR_USAGE = 64,
            EXIT_ERR_SYNTAX = 65,
            EXIT_ERR_FILE = 72,
        }

        private static readonly Interpreter Interpreter = new Interpreter();
        public static string CurrentFile = "<stdin>";

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                StdError.ThrowArgumentError();
            }
            else if (args.Length == 1)
            {
                RunFromFile(args[0]);
            }
            else
            {
                RunFromREPL();
            }
        }

        private static void RunFromFile(string filedir)
        {
            if (File.Exists(filedir))
            {
                CurrentFile = Path.GetFullPath(filedir);
                byte[] bytes = File.ReadAllBytes(filedir);
                string source = Encoding.UTF8.GetString(bytes);
                Execute(source);

                if (StdError.HasSyntaxError) { System.Environment.Exit((int) ExitType.EXIT_ERR_SYNTAX); }
                if (StdError.HasRuntimeError) { System.Environment.Exit((int) ExitType.EXIT_ERR_FILE); }
            } 
            else
            {
                StdError.ThrowFileNotFound(filedir);
            }
        }

        private static void RunFromREPL()
        {
            Console.WriteLine(Info.REPLMessage());
            StreamReader reader = new StreamReader(Console.OpenStandardInput());

            for (;;)
            {
                // Main loop just attends to expressions inputted by the user.
                Console.Write(">>> ");
                string line = reader.ReadLine();
                
                // Once we find a valid opening, all statements are now allowed.
                // Since a CODE source code is structured as a one big block of statement, we'll have another loop listening for it.
                if (line == "BEGIN CODE")
                {
                    string source = "BEGIN CODE";
                    for (;;)
                    {
                        Console.Write("... ");
                        string srcLine = reader.ReadLine();

                        source += System.Environment.NewLine + srcLine;
                        if (srcLine == "END CODE") { break; }
                    }
                    Execute(source);
                    StdError.HasSyntaxError = false;
                }

                if (line == null) break;

                //Execute(line);
                //StdError.HasSyntaxError = false;
            }
        }

        private static void Execute(string source)
        {
            List<Token> tokens = new Lexer(source).GenerateTokenStream();
            Parser parser = new Parser(tokens);
            List<Statement> statements = parser.Parse();

            if (StdError.HasSyntaxError) return;
            Interpreter.Interpret(statements);
        }
    }
}
