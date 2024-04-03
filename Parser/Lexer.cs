using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CODE_interpreter.Parser
{
    public class Lexer
    {
        private string Source { get; }
        private List<Token> Tokens = new List<Token>();

        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static readonly Dictionary<string, Token.Type> reservedKeywords = new Dictionary<string, Token.Type>
        {
            {"CODE", Token.Type.CODE},
            {"BEGIN CODE", Token.Type.BEGIN_CODE},
            {"END CODE", Token.Type.END_CODE},
            {"BEGIN", Token.Type.BLOCK_START},
            {"END", Token.Type.BLOCK_END},
            {"INT", Token.Type.INT},
            {"CHAR", Token.Type.CHAR},
            {"BOOL", Token.Type.BOOL},
            {"FLOAT", Token.Type.FLOAT},
            {"IF", Token.Type.IF},
            {"ELSE", Token.Type.ELSE},
            {"ELSE IF", Token.Type.ELIF},
            {"BEGIN IF", Token.Type.BEGINIF},
            {"END IF", Token.Type.ENDIF},
            {"WHILE", Token.Type.WHILE},
            {"BEGIN WHILE", Token.Type.BEGIN_WHILE},
            {"END WHILE", Token.Type.END_WHILE},
            {"DISPLAY", Token.Type.DISPLAY},
            {"SCAN", Token.Type.SCAN},
            {"AND", Token.Type.AND},
            {"OR", Token.Type.OR},
            {"NOT", Token.Type.NOT},
        };

        public Lexer(string source)
        {
            this.Source = source;
        }

        public List<Token> GenerateTokens()
        {
            while (!IsEndOfInput())
            {
                start = current;
                Tokenize();
            }
            Tokens.Add(new Token(Token.Type.EOF, "", null, line));
            return Tokens;
        }

        private void Tokenize()
        {
            char c = Advance();
            switch (c)
            {
                case '(':
                    AddToken(Token.Type.LEFT_PARANTHESIS);
                    break;
                case ')':
                    AddToken(Token.Type.RIGHT_PARENTHESIS);
                    break;
                case '[':
                    AddToken(Token.Type.LEFT_SBRACKET);
                    break;
                case ']':
                    AddToken(Token.Type.RIGHT_SBRACKET);
                    break;
                case ',':
                    AddToken(Token.Type.COMMA);
                    break;
                case ':':
                    AddToken(Token.Type.COLON);
                    break;
                case '+':
                    AddToken(Token.Type.ADD);
                    break;
                case '-':
                    AddToken(Token.Type.SUB);
                    break;
                case '*':
                    AddToken(Token.Type.MULT);
                    break;
                case '/':
                    AddToken(Token.Type.DIV);
                    break;
                case '%':
                    AddToken(Token.Type.MOD);
                    break;
                case '$':
                    AddToken(Token.Type.NEWLINE);
                    break;
                case '&':
                    AddToken(Token.Type.CONCAT);
                    break;
                case '#':
                    while (LookAhead() != '\n' && !IsEndOfInput()) { Advance(); }
                    break;
                case '=':
                    AddToken(Match('=') ? Token.Type.EQUAL : Token.Type.ASSIGNMENT);
                    break;
                case '>':
                    AddToken(Match('=') ? Token.Type.GREATER_EQUAL : Token.Type.GREATER);
                    break;
                case '<':
                    if (Match('>'))
                    {
                        AddToken(Token.Type.NOT_EQUAL);
                    } 
                    else if (Match('='))
                    {
                        AddToken(Token.Type.LESSER_EQUAL);
                    }
                    else
                    {
                        AddToken(Token.Type.LESSER);
                    }
                    break;
                case '"':
                    GetStringLiteral();
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                default:
                    if (IsAlpha(c))
                    {
                        TokenizeIdentifier();
                    }
                    else if (IsDigit(c))
                    {
                        TokenizeNumber();
                    }
                    else
                    {
                        CODE.ThrowError(line, "Unexpected character");
                    }
                    
                    break;

            }
        }

        private void TokenizeIdentifier()
        {
            while (IsAlphaNumeric(LookAhead())) { Advance(); }
            string text = Source.Substring(start, current - start);
            Token.Type type = reservedKeywords.TryGetValue(text, out Token.Type value) ? value : Token.Type.IDENTIFIER;
            AddToken(type);
        }

        private void TokenizeNumber()
        {
            while (IsDigit(LookAhead())) Advance();

            if (LookAhead() == '.' && IsDigit(LookAheadNext()))
            {
                Advance();

                while (IsDigit(LookAhead())) Advance();
            }

            AddToken(Token.Type.FLOAT, double.Parse(Source.Substring(start, current - start)));
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
        // Scan and consume the character at current pointer.
        private char Advance()
        {
            return Source[current++];
        }

        // Scan and consume the character if and only if it matches correct pattern.
        private bool Match(char expectedChar)
        {
            if (IsEndOfInput()) return false;
            if (Source[current] != expectedChar) { return false; }

            current++;
            return true;
        }

        // Scan but don't consume the character.
        private char LookAhead()
        {
            if (IsEndOfInput()) { return '\0'; }
            return Source[current];
        }

        private char LookAheadNext()
        {
            if (current + 1 >= Source.Length) { return '\0'; }
            return Source[current + 1];
        }

        private void AddToken(Token.Type type)
        {
            AddToken(type, null);
        }

        private void AddToken(Token.Type type, object literal)
        {
            string text = Source.Substring(start, current - start);
            Tokens.Add(new Token(type, text, literal, line));
        }

        private void GetStringLiteral()
        {
            while (LookAhead() != '"' && !IsEndOfInput())
            {
                if (LookAhead() == '\n') { line++; }
                Advance();
            }

            if (IsEndOfInput())
            {
                CODE.ThrowError(line, "Unterminated string");
                return;
            }

            Advance();
            string value = Source.Substring(start + 1, current - 1 - start - 1);
            AddToken(Token.Type.STRING, value);
        }

        private bool IsEndOfInput()
        {
            return current >= Source.Length;
        }
    }
}
