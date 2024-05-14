using System;

using CODEInterpreter.Strings;
using CODEInterpreter.Analyzers;

namespace CODEInterpreter.Errors
{
    public static class StdError
    {
        public class ParseError : Exception
        {
            public ParseError() : base() { }
            public ParseError(string message) : base(message) { }
            public ParseError(string message, Exception inner) : base(message, inner) { }
        }
        public class RuntimeError : Exception
        {
            public Token Token { get; }
            public RuntimeError(Token token, string message) : base(message)
            {
                Token = token;
            }
        }

        private static bool hasRaisedSyntaxError = false;
        private static bool hasRaisedRuntimeError = false;

        private static void BroadcastError(int line, string where, string message)
        {
            Console.WriteLine($"In file \"{CODE.CurrentFile}\" [Line {line}]");
            Console.Error.WriteLine($"Error {where}: {message}");
            HasSyntaxError = true;
        }
        public static void ThrowLexerError(int line, string message)
        {
            BroadcastError(line, "", message);
        }
        public static void ThrowSyntaxError(Token token, string type, string message)
        {

        }
        public static ParseError ThrowParseError(Token token, string message)
        {
            if (token.TokenType == Token.Type.EOF)
            {
                BroadcastError(token.Line, "at end", message);
            }
            else
            {
                BroadcastError(token.Line, "at '" + token.Lexeme + "'", message);
            }
            return new ParseError();
        }
        public static void ThrowRuntimeError(StdError.RuntimeError error)
        {
            Console.Error.WriteLine($"In file \"{CODE.CurrentFile}\" [Line {error.Token.Line}]\n{error.Message} ");
            HasRuntimeError = true;
        }
        public static void ThrowArgumentError()
        {
            Console.Error.WriteLine(UsageError.ArgumentError());
            System.Environment.Exit((int) CODE.ExitType.EX_ARGS_ERR);
        }
        public static void ThrowFileNotFound(string filepath)
        {
            Console.Error.WriteLine(UsageError.FileNotFoundError(filepath));
            System.Environment.Exit((int) CODE.ExitType.EXIT_ERR_FILE);
        }
        public static bool HasSyntaxError
        {
            get { return hasRaisedSyntaxError; }
            set { hasRaisedSyntaxError = value; }
        }
        public static bool HasRuntimeError
        {
            get { return hasRaisedRuntimeError; }
            set { hasRaisedRuntimeError = value; }
        }
    }
}
