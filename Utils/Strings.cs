namespace CODEInterpreter.Strings
{
    public static class Info
    {
        private const string REPLHeader = "CODE 0.5.1-alpha by Hitsuji Labs - https://github.com/ShySpaceSheep/CODE-interpreter";
        private const string REPLSubheader = "Type \"help\" or \"credits\" for more information.";
        public static string REPLMessage() { return $"{REPLHeader}\n{REPLSubheader}"; }
    }

    public static class UsageError
    {
        private const string ArgErrHeader = "Invalid arguments, refer below to get started";
        private const string ArgErrSubheader = "Usage: cde [Source] or cde.exe [Source]";
        private const string FileErrHeader = "Can't open file: ";
        private const string FileErrSubheader = "Please make sure file or directory exists";

        public static string ArgumentError() { return $"{ArgErrHeader}\n{ArgErrSubheader}"; }
        public static string FileNotFoundError(string filepath) { return $"{FileErrHeader}: {filepath}\n{FileErrSubheader}"; }
    }

    public static class SyntaxError
    {
        private const string ErrorType = "SyntaxError:";
        private const string UnexpectedChara = "Unexpected character";

        public static string UnexpectedCharacter() { return $"{ErrorType} {UnexpectedChara}"; }
    }

    public static class NameError
    {
        private const string ErrorType = "NameError:";
        private const string Undefined = "is not defined";
        private const string Redefined = "is already defined with type 'INT'";

        public static string UndefinedVar(string identifier)
        {
            return $"{ErrorType} Identifier '{identifier}' {Undefined}";
        }
        public static string RedefinedVar(string identifier)
        {
            return $"{ErrorType} Identifier '{identifier}' {Redefined}";
        }
    }

    public static class TypeError
    {
        private const string ErrorType = "TypeError:";
        
        public static string IncompatibleType(string identifier, string type)
        {
            return $"{ErrorType} Identifier '{identifier}' of type {type} expected correct value.";
        }
    }
}
