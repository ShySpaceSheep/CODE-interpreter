﻿namespace CODEInterpreter.Analyzers
{
    public class Token
    {
        public enum Type
        {
            // Reserved structure/control keywords
            CODE, BLOCK_START, BLOCK_END, EOF,
            IF, ELIF, ELSE, WHILE,
            BREAK, CONTINUE,
            
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
            LEFT_PARENTHESIS, RIGHT_PARENTHESIS,
            LEFT_SBRACKET, RIGHT_SBRACKET,

            // Keyword for data types and literals
            IDENTIFIER, VAL_INT, VAL_FLOAT, VAL_CHAR, STRING, ASSIGNABLE,
            TRUE, FALSE, NIL,
            VAR_INT, VAR_FLOAT, VAR_CHAR, VAR_BOOL,

            // Misc
            ENDLINE, COMMA, COLON,
        }

        public Type TokenType { get; }
        public string Lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        public Token(Type type, string lexeme, object literal, int line)
        {
            TokenType = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }
    }
}
