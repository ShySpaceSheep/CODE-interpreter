using System.Collections.Generic;

using CODEInterpreter.Errors;
using CODEInterpreter.Strings;

using static CODEInterpreter.Analyzers.Token.Type;

namespace CODEInterpreter.Analyzers
{
    public class Lexer
    {
        private readonly string _source;
        private readonly List<Token> _tokenStream = new();

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        private int _column = 1;

        public Lexer(string source)
        { 
            _source = source;
        }

        public List<Token> GenerateTokenStream()
        {
            while (!IsAtEndOfFile())
            {
                _start = _current;
                Tokenize();
            }

            _tokenStream.Add(new Token(EOF, "", null, _line));
            return _tokenStream;
        }

        private void Tokenize()
        {
            char c = Advance();
            _column++;
            switch (c)
            {
                case '+':
                    AddToken(ADD);
                    break;
                case '-':
                    AddToken(SUB);
                    break;
                case '*':
                    AddToken(MULT);
                    break;
                case '/':
                    AddToken(DIV);
                    break;
                case '%':
                    AddToken(MOD);
                    break;
                case '(':
                    AddToken(LEFT_PARENTHESIS);
                    break;
                case ')':
                    AddToken(RIGHT_PARENTHESIS);
                    break;
                case ',':
                    AddToken(COMMA);
                    break;
                case ':':
                    AddToken(COLON);
                    break;
                case '$':
                    AddToken(STRING, "\n");
                    break;
                case '&':
                    AddToken(CONCAT);
                    break;
                case '=':
                    AddToken(Match('=') ? EQUAL : ASSIGNMENT);
                    break;
                case '>':
                    AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '<':
                    if (Match('>'))
                    {
                        AddToken(NOT_EQUAL);
                        break;
                    }
                    AddToken(Match('=') ? LESSER_EQUAL : LESSER);
                    break;
                case '#':
                    IgnoreLine();
                    break;
                case '[':
                    EscapeCharacter();
                    break;
                case '"':
                    GetStringLiteral();
                    break;
                case '\'':
                    GetCharLiteral();
                    break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    //AddToken(ENDLINE);
                    _column = 0;
                    _line++;
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
                        StdError.ThrowLexerError(_line, SyntaxError.UnexpectedCharacter());
                    }
                    break;
            }
        }

        private void TokenizeIdentifier()
        {
            while (IsAlphaNumeric(LookAhead())) { Advance(); }
            string text = _source[_start.._current];
            Token.Type type = Keywords.GetReservedKeywords().TryGetValue(text, out Token.Type value) ? value : IDENTIFIER;
            AddToken(type);
        }

        private void TokenizeNumber()
        {
            bool isInteger = true;
            while (IsDigit(LookAhead())) { Advance(); }

            if (LookAhead() == '.' && IsDigit(LookAheadNext()))
            {
                Advance();
                isInteger = false;
                while (IsDigit(LookAhead())) { Advance(); }
            }

            Token.Type type = isInteger ? VAL_INT : VAL_FLOAT;
            AddToken(type, isInteger ? int.Parse(_source[_start.._current]) : double.Parse(_source[_start.._current]));
        }

        private void GetStringLiteral()
        {
            while (LookAhead() != '"' && !IsAtEndOfFile())
            {
                if (LookAhead() == '\n') { _line++; }
                Advance();
            }

            if (IsAtEndOfFile())
            {
                StdError.ThrowLexerError(_line, "Unterminated string");
                return;
            }

            Advance();
            string value = _source.Substring(_start + 1, _current - 1 - _start - 1);

            // There's a specific weird choice about choosing to do boolean values within string constraints.
            // Make an exception for literals who simply identify as "TRUE" and "FALSE".
            // This will be changed in the 1.0 release of CODE.
            if (value == "TRUE")
            {
                AddToken(TRUE);
            }
            else if (value == "FALSE")
            {
                AddToken(FALSE);
            }
            else
            {
                AddToken(STRING, value);
            }
        }

        // TODO
        private void GetCharLiteral()
        {
            if (LookAhead() == '\'' && !IsAtEndOfFile())
            {
                Advance();
                StdError.ThrowLexerError(_line, "Empty char constant");
                return;
            }

            char value = _source[_current];
            AddToken(VAL_CHAR, value);
            Advance();

            if (Advance() != '\'')
            {
                StdError.ThrowLexerError(_line, "Expected terminator for char constant");
                return;
            }
        }

        private void EscapeCharacter()
        {
            while (LookAhead() != ']' && !IsAtEndOfFile())
            {
                if (LookAhead() == '\n') { _line++; }
                Advance();
            }

            if (IsAtEndOfFile())
            {
                StdError.ThrowLexerError(_line, "Unterminated escape string");
                return;
            }

            Advance();
            string value = _source.Substring(_start + 1, _current - 1 - _start - 1);
            AddToken(STRING, value);
        }

        private void IgnoreLine()
        {
            while (LookAhead() != '\n' && !IsAtEndOfFile()) { Advance(); }
        }

        private void AddToken(Token.Type type)
        {
            AddToken(type, null);
        }

        private void AddToken(Token.Type type, object literal)
        {
            string text = _source[_start.._current];
            _tokenStream.Add(new Token(type, text, literal, _line));
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private bool Match(char expectedChar)
        {
            if (IsAtEndOfFile()) return false;
            if (_source[_current] != expectedChar) { return false; }

            _current++;
            return true;
        }

        private char LookAhead()
        {
            if (IsAtEndOfFile()) { return '\0'; }
            return _source[_current];
        }

        private char LookAheadNext()
        {
            if (_current + 1 >= _source.Length) { return '\0'; }
            return _source[_current + 1];
        }

        private bool IsAtEndOfFile()
        {
            return _current >= _source.Length;
        }

        private static bool IsAlpha(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private static bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}
