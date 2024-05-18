using System;
using System.Collections.Generic;

using CODEInterpreter.Analyzers;
using CODEInterpreter.Errors;
using CODEInterpreter.Strings;

namespace CODEInterpreter
{
    public class Environment
    {
        private readonly Dictionary<string, Token.Type> _typeMemory = new();
        private readonly Dictionary<string, object> _dataMemory = new();

        public void DefineVar(Token.Type dataType, Token name, Object value)
        {
            if (_dataMemory.ContainsKey(name.Lexeme))
            {
                throw new StdError.RuntimeError(name, NameError.RedefinedVar(name.Lexeme));
            }
            _typeMemory.Add(name.Lexeme, dataType);
            _dataMemory.Add(name.Lexeme, value);
        }

        public void Assign(Token name, Object value)
        {
            if (_typeMemory.TryGetValue(name.Lexeme, out Token.Type varDataType))
            {
                if (!IsCompatibleDataType(varDataType, value)) { 
                    throw new StdError.RuntimeError(name, TypeError.IncompatibleType(name.Lexeme, varDataType.ToString()));
                }
                _dataMemory[name.Lexeme] = value;
                return;
            }

            throw new StdError.RuntimeError(name, NameError.UndefinedVar(name.Lexeme));
        }

        public object Get(Token name)
        {
            if (_dataMemory.TryGetValue(name.Lexeme, out object value)) { return value; }
            throw new StdError.RuntimeError(name, NameError.UndefinedVar(name.Lexeme));
        }

        private bool IsCompatibleDataType(Token.Type dataType, Object value)
        {
            switch (dataType)
            {
                case Token.Type.VAR_INT:
                    return value is int;
                case Token.Type.VAR_FLOAT:
                    return value is double;
                case Token.Type.VAR_CHAR:
                    return value is char;
                case Token.Type.VAR_BOOL:
                    return value is bool;
                default:
                    return false;
            }
        }
    }
}
