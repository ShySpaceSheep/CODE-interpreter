using System.Collections.Generic;

using CODE_interpreter.AST;

namespace CODE_interpreter.Analyzers
{
    public class Parser
    {
        private readonly List<Token> TokenStream;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.TokenStream = tokens;
        }

        public List<Statement> Parse()
        {
            List<Statement> statements = new List<Statement>();

            try
            {
                // Just look at the TokenStream directly ahead of time if it's in a valid block.
                Consume(Token.Type.BLOCK_START, "SyntaxError: Expected 'BEGIN' block.");
                Consume(Token.Type.CODE, "SyntaxError: Expected 'CODE' block type.");

                if (TokenStream[TokenStream.Count - 2].TokenType != Token.Type.CODE)
                {
                    if (TokenStream[TokenStream.Count - 3].TokenType != Token.Type.BLOCK_END)
                    {
                        throw StdError.ThrowParseError(Previous(), "SyntaxError: Unterminated 'CODE' block.");
                    }
                    throw StdError.ThrowParseError(Previous(), "SyntaxError: Expected 'CODE' block type.");
                }

                TokenStream.RemoveAt(TokenStream.Count - 2);
                TokenStream.RemoveAt(TokenStream.Count - 2);
                
                while (!IsAtEnd())
                {
                    statements.Add(Declaration());
                }
                
                return statements;
            }
            catch (StdError.ParseError e)
            {
                Synchronize();
                return null;
            }
        }

        private Expression Expression()
        {
            return Assignment();
        }

        private Statement Declaration()
        {
            try
            {
                if (Match(Token.Type.VAR_INT)) return IntVarDeclaration();
                if (Match(Token.Type.VAR_FLOAT)) return FloatVarDeclaration();
                if (Match(Token.Type.VAR_CHAR)) return CharVarDeclaration();
                if (Match(Token.Type.VAR_BOOL)) return BoolVarDeclaration();
                return Statement();
            }
            catch (StdError.ParseError e)
            {
                Synchronize();
                return null;
            }
        }

        private Statement Statement()
        {
            if (Match(Token.Type.IF)) return IfStatement();
            if (Match(Token.Type.WHILE)) return WhileStatement();
            if (Match(Token.Type.DISPLAY))
            {
                if (Match(Token.Type.COLON)) return PrintStatement();
                throw StdError.ThrowParseError(Previous(), "Expected ':'");
            }
            /*
            if (Match(Token.Type.SCAN))
            {
                if (Match(Token.Type.COLON)) return ScanStatement();
                throw StdError.ThrowParseError(Previous(), "Expected ':'");
            }*/ 

            return ExpressionStatement();
        }

        private Statement IfStatement()
        {
            Consume(Token.Type.LEFT_PARENTHESIS, "Expect '(' after 'if'.");
            Expression condition = Expression();
            Consume(Token.Type.RIGHT_PARENTHESIS, "Expect ')' after 'if'.");


            List<Statement> thenBranch = IfBlock();
            List<Statement> elseBranch = null;

            if (Match(Token.Type.ELSE))
            {
                elseBranch = IfBlock();
            }

            return new Statement.If(condition, thenBranch, elseBranch);
        }

        private Statement PrintStatement()
        {
            Expression value = Expression();
            return new Statement.Print(value);
        }

        /*private Statement ScanStatement()
        {

        }*/

        private Statement IntVarDeclaration()
        {
            List<Statement.Var> declarations_list = new List<Statement.Var>();
            for(; ; )
            {
                Token name = Consume(Token.Type.IDENTIFIER, "Expect variable name.");

                Expression initializer = null;
                if (Match(Token.Type.ASSIGNMENT))
                {
                    initializer = IntExpression();
                }

                declarations_list.Add(new Statement.Var(name, initializer));
                if (Match(Token.Type.COMMA))
                {
                    continue;
                }

                break;
            }
            return new Statement.VarList(declarations_list);
        }

        private Statement FloatVarDeclaration()
        {
            List<Statement.Var> declarations_list = new List<Statement.Var>();
            for (; ; )
            {
                Token name = Consume(Token.Type.IDENTIFIER, "Expect variable name.");

                Expression initializer = null;
                if (Match(Token.Type.ASSIGNMENT))
                {
                    initializer = FloatExpression();
                }
                declarations_list.Add(new Statement.Var(name, initializer));
                if (Match(Token.Type.COMMA))
                {
                    continue;
                }

                break;
            }
            return new Statement.VarList(declarations_list);
        }

        private Statement CharVarDeclaration()
        {
            List<Statement.Var> declarations_list = new List<Statement.Var>();
            for (; ; )
            {
                Token name = Consume(Token.Type.IDENTIFIER, "Expect variable name.");

                Expression initializer = null;
                if (Match(Token.Type.ASSIGNMENT))
                {
                    initializer = CharExpression();
                }
                declarations_list.Add(new Statement.Var(name, initializer));
                if (Match(Token.Type.COMMA))
                {
                    continue;
                }

                break;
            }
            return new Statement.VarList(declarations_list);
        }

        private Statement BoolVarDeclaration()
        {
            List<Statement.Var> declarations_list = new List<Statement.Var>();
            for (; ; )
            {
                Token name = Consume(Token.Type.IDENTIFIER, "Expect variable name.");

                Expression initializer = null;
                if (Match(Token.Type.ASSIGNMENT))
                {
                    initializer = BoolExpression();
                }
                declarations_list.Add(new Statement.Var(name, initializer));
                if (Match(Token.Type.COMMA))
                {
                    continue;
                }

                break;
            }
            return new Statement.VarList(declarations_list);
        }

        private Statement WhileStatement()
        {
            Consume(Token.Type.LEFT_PARENTHESIS, "Expect '(' after 'while'.");
            Expression condition = Expression();
            Consume(Token.Type.RIGHT_PARENTHESIS, "Expect ')' after condition.");
            List<Statement> body = WhileBlock();

            return new Statement.While(condition, body);
        }

        private Statement ExpressionStatement()
        {
            Expression expr = Expression();
            return new Statement.Expression(expr);
        }

        private List<Statement> IfBlock()
        {
            List<Statement> statements = new List<Statement>();
            Consume(Token.Type.BLOCK_START, "SyntaxError: Expected IF block.");
            Consume(Token.Type.IF, "SyntaxError: Expected IF block");

            while (!Check(Token.Type.BLOCK_END) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(Token.Type.BLOCK_END, "SyntaxError: Unterminated IF block.");
            Consume(Token.Type.IF, "SyntaxError: Unterminated IF block.");

            return statements;
        }

        private List<Statement> WhileBlock()
        {
            List<Statement> statements = new List<Statement>();
            Consume(Token.Type.BLOCK_START, "SyntaxError: Expected WHILE block.");
            Consume(Token.Type.WHILE, "SyntaxError: Expected WHILE block.");

            while (!Check(Token.Type.BLOCK_END) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(Token.Type.BLOCK_END, "SyntaxError: Unterminated WHILE block.");
            Consume(Token.Type.WHILE, "SyntaxError: Unterminated WHILE block.");

            return statements;
        }

        private Expression Assignment()
        {
            Expression expr = OrStatement();
            if (Match(Token.Type.ASSIGNMENT))
            {
                Token equals = Previous();
                Expression value = Assignment();

                if (expr is Expression.Variable) {
                    Token name = ((Expression.Variable)expr).Name;
                    return new Expression.Assign(name, value);
                }

                StdError.ThrowParseError(equals, "Invalid assignment target.");
            }
            return expr;
        }

        private Expression OrStatement()
        {
            Expression expr = AndStatement();
            while (Match(Token.Type.OR))
            {
                Token op = Previous();
                Expression right = AndStatement();
                expr = new Expression.Logical(expr, op, right);
            }
            return expr;
        }

        private Expression AndStatement()
        {
            Expression expr = Equality();

            while (Match(Token.Type.AND))
            {
                Token op = Previous();
                Expression right = Equality();
                expr = new Expression.Logical(expr, op, right);
            }

            return expr;
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

            throw StdError.ThrowParseError(LookAhead(), message);
        }

        private bool Check(Token.Type type)
        {
            if (IsAtEnd()) return false;
            return (LookAhead().TokenType == type);
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return (LookAhead().TokenType == Token.Type.EOF);
        }

        private Token LookAhead()
        {
            return TokenStream[current];
        }

        private Token Previous()
        {
            return TokenStream[current - 1];
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().TokenType == Token.Type.EOF) { return; }
                switch(LookAhead().TokenType)
                {
                    case Token.Type.VAR_INT:
                    case Token.Type.VAR_FLOAT:
                    case Token.Type.VAR_CHAR:
                    case Token.Type.VAR_BOOL:
                    case Token.Type.IF:
                    case Token.Type.WHILE:
                    case Token.Type.DISPLAY:
                    case Token.Type.SCAN:
                        return;
                }
                Advance();
            }
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

            while (Match(Token.Type.SUB, Token.Type.ADD, Token.Type.CONCAT, Token.Type.MOD))
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

        private Expression IntExpression()
        {
            if (Match(Token.Type.VAL_INT))
            {
                return new Expression.Literal(Previous().Literal);
            }

            throw StdError.ThrowParseError(LookAhead(), "Expected int expression.");
        }

        private Expression FloatExpression()
        {
            if (Match(Token.Type.VAL_FLOAT))
            {
                return new Expression.Literal(Previous().Literal);
            }

            throw StdError.ThrowParseError(LookAhead(), "Expected float expression.");
        }

        private Expression CharExpression()
        {
            if (Match(Token.Type.VAL_CHAR))
            {
                return new Expression.Literal(Previous().Literal);
            }

            throw StdError.ThrowParseError(LookAhead(), "Expected char expression.");
        }

        private Expression BoolExpression()
        {
            if (Match(Token.Type.FALSE)) return new Expression.Literal(false);
            if (Match(Token.Type.TRUE)) return new Expression.Literal(true);

            throw StdError.ThrowParseError(LookAhead(), "Expected boolean expression.");
        }

        private Expression Primary()
        {
            if (Match(Token.Type.FALSE)) return new Expression.Literal(false);
            if (Match(Token.Type.TRUE)) return new Expression.Literal(true);
            if (Match(Token.Type.NIL)) return new Expression.Literal(null);

            if (Match(Token.Type.VAL_INT, Token.Type.VAL_FLOAT, Token.Type.VAL_CHAR, Token.Type.STRING))
            {
                return new Expression.Literal(Previous().Literal);
            }

            if (Match(Token.Type.IDENTIFIER))
            {
                return new Expression.Variable(Previous());
            }

            if (Match(Token.Type.LEFT_PARENTHESIS))
            {
                Expression expr = Expression();
                Consume(Token.Type.RIGHT_PARENTHESIS, "Expect ')' after expression.");
                return new Expression.Grouping(expr);
            }

            throw StdError.ThrowParseError(LookAhead(), "Expect expression.");
        }
    }
}
