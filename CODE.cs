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

        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine(CODEStrings.InvalidUsage);
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
            } 
            else
            {
                Console.WriteLine(CODEStrings.FileNotFoundL1 + filePath + "\n" + CODEStrings.FileNotFoundL2);
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
            }
        }

        private static void ExecuteLine(string line)
        {
            List<Token> tokens = Tokenize(line);
            foreach (Token t in tokens)
            {
                Console.WriteLine(t);
            }
        }

        private static List<Token> Tokenize(string source)
        {
            List<Token> tokens = new List<Token>();
            string[] words = source.Split(' ');
            foreach (string word in words)
            {
                tokens.Add(new Token(word));
            }
            return tokens;
        }
    }
}
