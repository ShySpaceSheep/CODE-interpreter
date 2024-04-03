using CODE_interpreter.AST;
using System;
using System.Collections.Generic;

namespace CODE_interpreter.Parser
{
    public class Parser
    {
        public class ParseError : Exception
        {
            public ParseError() : base() { }

            public ParseError(string message) : base(message) { }

            public ParseError(string message, Exception inner) : base(message, inner) { }
        }

        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expression parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        private Expression Expression()
        {
            return Equality();
        }

        private Expression Equality()
        {
            Expression expr = Comparison();
            while (Match(Token.Type.NOT_EQUAL, Token.Type.EQUAL))
            {
                Token op = Previous();
                Expression right = Comparison();
                expr = new Expression.Binary(expr, op, right);
            }
            return expr;
        }

        private bool Match(params Token.Type[] types)
        {
            foreach (Token.Type type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Consume(Token.Type type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(LookAhead(), message);
        }

        private bool Check(Token.Type type)
        {
            if (IsAtEnd()) return false;
            return (LookAhead().CurrType == type);
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return (LookAhead().CurrType == Token.Type.EOF);
        }

        private Token LookAhead()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private ParseError Error(Token token, string message)
        {
            CODE.Error(token, message);
            return new ParseError();
        }

        private Expression Comparison()
        {
            Expression expr = Term();

            while (Match(Token.Type.GREATER, Token.Type.GREATER_EQUAL, Token.Type.LESSER, Token.Type.LESSER_EQUAL))
            {
                Token op = Previous();
                Expression right = Term();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        private Expression Term()
        {
            Expression expr = Factor();

            while (Match(Token.Type.SUB, Token.Type.ADD, Token.Type.CONCAT))
            {
                Token op = Previous();
                Expression right = Factor();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        private Expression Factor()
        {
            Expression expr = Unary();

            while (Match(Token.Type.DIV, Token.Type.MULT))
            {
                Token op = Previous();
                Expression right = Unary();
                expr = new Expression.Binary(expr, op, right);
            }

            return expr;
        }

        private Expression Unary()
        {
            if (Match(Token.Type.ADD, Token.Type.SUB))
            {
                Token op = Previous();
                Expression right = Unary();
                return new Expression.Unary(op, right);
            }

            return Primary();
        }

        private Expression Primary()
        {
            if (Match(Token.Type.FALSE)) return new Expression.Literal(false);
            if (Match(Token.Type.TRUE)) return new Expression.Literal(true);
            if (Match(Token.Type.NIL)) return new Expression.Literal(null);

            if (Match(Token.Type.FLOAT, Token.Type.STRING))
            {
                return new Expression.Literal(Previous().Literal);
            }

            if (Match(Token.Type.LEFT_PARANTHESIS))
            {
                Expression expr = Expression();
                Consume(Token.Type.RIGHT_PARENTHESIS, "Expect ')' after expression.");
                return new Expression.Grouping(expr);
            }

            throw Error(LookAhead(), "Expect expression.");
        }
    }
}
