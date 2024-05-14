using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using CODEInterpreter.AST;
using CODEInterpreter.Strings;
using CODEInterpreter.Analyzers;
using CODEInterpreter.Errors;

namespace CODEInterpreter
{
    public class CODE
    {
        [Flags]
        public enum ExitType: int
        {
            EX_SUCCESS = 0,
            EX_ARGS_ERR = 64,
            EX_SYNTAX_ERR = 65,
            EX_FILE_ERR = 69,
            EXIT_ERR_FILE = 72,
        }

        private static readonly Interpreter _interpreter = new();
        private static string _currentFile = "<stdin>";

        public static int Main(string[] args)
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

            return (int) ExitType.EX_SUCCESS;
        }

        private static void RunFromFile(string filedir)
        {
            if (!File.Exists(filedir)) { StdError.ThrowFileNotFound(filedir); }

            _currentFile = Path.GetFullPath(filedir);
            byte[] bytes = File.ReadAllBytes(filedir);
            string source = Encoding.UTF8.GetString(bytes);
            Execute(source);

            if (StdError.HasSyntaxError) { System.Environment.Exit((int) ExitType.EX_SYNTAX_ERR); }
        }

        private static void RunFromREPL()
        {
            Console.WriteLine(Info.REPLMessage());
            StreamReader reader = new(Console.OpenStandardInput());

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
            Parser parser = new(tokens);
            List<Statement> statements = parser.Parse();

            if (StdError.HasSyntaxError) return;
            _interpreter.Interpret(statements);
        }

        public static string CurrentFile
        {
            get { return _currentFile; }
        }
    }
}
