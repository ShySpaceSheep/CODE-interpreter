using System;
using System.Collections.Generic;

using CODE_interpreter.CODEStrings;
using CODE_interpreter.Analyzers;

namespace CODE_interpreter
{
    public class Environment
    {
        private readonly Dictionary<string, object> VarMemory = new Dictionary<string, object>();

        public void DefineVar(Token name, Object value)
        {
            if (VarMemory.ContainsKey(name.Lexeme))
            {
                throw new StdError.RuntimeError(name, NameError.RedefinedVar(name.Lexeme));
            }
            VarMemory.Add(name.Lexeme, value);
        }
        public void Assign(Token name, Object value)
        {
            if (VarMemory.ContainsKey(name.Lexeme))
            {
                VarMemory[name.Lexeme] = value;
                return;
            }

            throw new StdError.RuntimeError(name, NameError.UndefinedVar(name.Lexeme));
        }
        public object Get(Token name)
        {
            if (VarMemory.TryGetValue(name.Lexeme, out object value)) { return value; }
            throw new StdError.RuntimeError(name, NameError.UndefinedVar(name.Lexeme));
        }
    }
}
