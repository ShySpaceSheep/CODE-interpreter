using System;
using System.Collections.Generic;

using CODEInterpreter.Analyzers;
using CODEInterpreter.Errors;
using CODEInterpreter.Strings;

namespace CODEInterpreter
{
    public class Environment
    {
        private readonly Dictionary<string, object> _memory = new();

        public void DefineVar(Token name, Object value)
        {
            if (_memory.ContainsKey(name.Lexeme))
            {
                throw new StdError.RuntimeError(name, NameError.RedefinedVar(name.Lexeme));
            }
            _memory.Add(name.Lexeme, value);
        }

        public void Assign(Token name, Object value)
        {
            if (_memory.ContainsKey(name.Lexeme))
            {
                _memory[name.Lexeme] = value;
                return;
            }
            throw new StdError.RuntimeError(name, NameError.UndefinedVar(name.Lexeme));
        }

        public object Get(Token name)
        {
            if (_memory.TryGetValue(name.Lexeme, out object value)) { return value; }
            throw new StdError.RuntimeError(name, NameError.UndefinedVar(name.Lexeme));
        }
    }
}
