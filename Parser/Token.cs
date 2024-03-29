using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_interpreter.Parser
{
    public class Token
    {
        public enum Type
        {
            // Reserved structure/control keywords
            CODE, BEGIN_CODE, END_CODE,
            IF, ELIF, ELSE,
            BEGINIF, ENDIF,
            WHILE, BEGIN_WHILE, END_WHILE,
            BLOCK_START, BLOCK_END, EOF,

            // Input/output keywords
            DISPLAY, SCAN,

            // Keywords for basic operators
            ADD, SUB, MULT, DIV, MOD,
            GREATER, LESSER,
            GREATER_EQUAL, LESSER_EQUAL,
            EQUAL, NOT_EQUAL,
            AND, OR, NOT,

            // Other available operators (single-character operators)
            NEWLINE, CONCAT, ASSIGNMENT,
            LEFT_PARAGRAPH, RIGHT_PARAGRAPH,
            LEFT_SBRACKET, RIGHT_SBRACKET,

            // Keyword for data types and literals
            IDENTIFIER,
            INT, CHAR, BOOL, FLOAT, STRING,

            // Misc
            COMMA, COLON
        }
        public Type CurrType { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        public Token(Type type, string lexeme, object literal, int line)
        {
            CurrType = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString()
        {
            return $"{CurrType} {Lexeme} {Literal}";
        }
    }
}
