using CODE_interpreter.AST;
using CODE_interpreter.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static CODE_interpreter.Interpreter;

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

        private static readonly Interpreter interpreter = new Interpreter();
        public static bool hasError = false;
        public static bool hadRuntimeError = false;

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.Error.WriteLine(CODEStrings.InvalidUsage);
                Environment.Exit((int) ExitType.EXIT_ERR_USAGE);
            }
            else if (args.Length == 1)
            {
                RunSourceFile(args[0]);
            }
            else
            {
                RunInteractive();
            }
        }

        private static void RunSourceFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                string content = Encoding.UTF8.GetString(bytes);
                ExecuteLine(content);

                if (hasError) { Environment.Exit((int) ExitType.EXIT_ERR_SYNTAX); }
                if (hadRuntimeError) { Environment.Exit((int)ExitType.EXIT_ERR_FILE); }
            } 
            else
            {
                Console.Error.WriteLine(CODEStrings.FileNotFoundL1 + filePath + "\n" + CODEStrings.FileNotFoundL2);
                Environment.Exit((int)ExitType.EXIT_ERR_FILE);
            }
        }

        private static void RunInteractive()
        {
            Console.WriteLine(CODEStrings.InterpreterREPL);
            StreamReader reader = new StreamReader(Console.OpenStandardInput());
            for (;;)
            {
                Console.Write(">>> ");
                string line = reader.ReadLine();
                if (line == null) break;
                ExecuteLine(line);
                hasError = false;
            }
        }

        private static void ExecuteLine(string source)
        {
            Lexer l = new Lexer(source);
            List<Token> tokens = l.GenerateTokens();

            CODE_interpreter.Parser.Parser parser = new CODE_interpreter.Parser.Parser(tokens);
            Expression expression = parser.parse();

            // Stop if there was a syntax error.
            if (hasError) return;

            interpreter.Interpret(expression);
        }

        public static void ThrowError(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hasError = true;
        }

        public static void Error(Token token, String message)
        {
            if (token.CurrType == Token.Type.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine(error.Message + "\n[line " + error.Token.Line + "]");
            hadRuntimeError = true;
        }
    }
}
