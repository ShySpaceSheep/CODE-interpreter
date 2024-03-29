using CODE_interpreter.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        public static bool hasError = false;

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

                if (hasError) { Environment.Exit((int)ExitType.EXIT_ERR_SYNTAX); }
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
            foreach (Token t in tokens)
            {
                Console.WriteLine(t);
            }
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
    }
}
