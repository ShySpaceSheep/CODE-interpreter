using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODEInterpreter.Analyzers
{
    public static class Keywords
    {
        private static readonly Dictionary<string, Token.Type> _rk = new()
        {
            {"CODE", Token.Type.CODE},
            {"BEGIN", Token.Type.BLOCK_START},
            {"END", Token.Type.BLOCK_END},
            {"INT", Token.Type.VAR_INT},
            {"CHAR", Token.Type.VAR_CHAR},
            {"BOOL", Token.Type.VAR_BOOL},
            {"FLOAT", Token.Type.VAR_FLOAT},
            {"IF", Token.Type.IF},
            {"ELSE", Token.Type.ELSE},
            {"WHILE", Token.Type.WHILE},
            {"DISPLAY", Token.Type.DISPLAY},
            {"SCAN", Token.Type.SCAN},
            {"AND", Token.Type.AND},
            {"OR", Token.Type.OR},
            {"NOT", Token.Type.NOT}
        };

        public static Dictionary<string, Token.Type> GetReservedKeywords() { return _rk; }
    }
}
