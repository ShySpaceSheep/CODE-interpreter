using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_interpreter
{
    public static class CODEStrings
    {
        public const string InvalidUsage = "Invalid arguments, refer below to get started\nUsage: cde [CODE source file] or cde.exe [CODE source file]";
        public const string FileNotFoundL1 = "Can't open file: ";
        public const string FileNotFoundL2 = "Please make sure file or directory exists";


        public const string InterpreterDesc = "CODE 0.1.0 by Hitsuji Labs - https://github.com/ShySpaceSheep/CODE-interpreter";
        public const string InterpreterHelp = "Type \"help\" or \"credits\" for more information.";
        public const string InterpreterREPL = InterpreterDesc + "\n" + InterpreterHelp;
    }
}
