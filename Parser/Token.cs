using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_interpreter.Parser
{
    public class Token
    {
        enum TokenType
        {
            // Reserved structure/control keywords
            TK_BEGINCDE, TK_ENDCDE,
            TK_IF, TK_ELIF, TK_ELSE,
            TK_BEGINIF, TK_ENDIF,
            TK_WHILE, TK_BEGINWH, TK_ENDWH,
            TK_COMMENT, TK_EOF,

            // Input/output keywords
            TK_DISPLAY, TK_SCAN,

            // Keywords for basic operators
            TK_ADD, TK_SUB, TK_MULT, TK_DIV, TK_MOD,
            TK_GREATER, TK_LESSER,
            TK_GEQUAL, TK_LEQUAL,
            TK_EQUAL, TK_NOEQUAL,
            TK_AND, TK_OR, TK_NOT,

            // Other available operators (single-character operators)
            TK_NEWLINE, TK_CONCAT,
            TK_LPARA, TK_RPARA,
            TK_LBRACKET, TK_RBRACKET,

            // Keyword for data types and literals
            TK_IDENTIFIER,
            TK_INT, TK_CHAR, TK_BOOL, TK_FLOAT,
        }
        public string Value { get; set; }

        public Token(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
