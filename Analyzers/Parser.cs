using System.Collections.Generic;

using CODEInterpreter.AST;
using CODEInterpreter.Errors;

using static CODEInterpreter.Analyzers.Token.Type;

namespace CODEInterpreter.Analyzers
{
    public class Parser
    {
        private readonly List<Token> _tokenStream;
        private int _current = 0;

        public Parser(List<Token> tokens)
        {
            this._tokenStream = tokens;
        }

        public List<Statement> Parse()
        {
            List<Statement> statements = new();
            try
            {
                // Just look at the TokenStream directly ahead of time if it's in a valid block.
                Consume(BLOCK_START, "SyntaxError: Expected 'BEGIN' block.");
                Consume(Token.Type.CODE, "SyntaxError: Expected 'CODE' block type.");

                if (_tokenStream[^2].TokenType != Token.Type.CODE)
                {
                    if (_tokenStream[^3].TokenType != BLOCK_END)
                    {
                        throw StdError.ThrowParseError(Previous(), "SyntaxError: Unterminated 'CODE' block.");
                    }
                    throw StdError.ThrowParseError(Previous(), "SyntaxError: Expected 'CODE' block type.");
                }

                _tokenStream.RemoveAt(_tokenStream.Count - 2);
                _tokenStream.RemoveAt(_tokenStream.Count - 2);
                
                while (!IsAtEndOfStream())
                {
                    statements.Add(Declaration());
                }
                
                return statements;
            }
            catch (StdError.ParseError)
            {
                Synchronize();
                return null;
            }
        }

        private void Synchronize()
        {
            Advance();
            while (!IsAtEndOfStream())
            {
                if (Previous().TokenType == Token.Type.EOF) { return; }
                switch (LookAhead().TokenType)
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

        private Statement PrintStatement()
        {
            Expression value = Expression();
            return new Statement.Print(value);
        }

        private Statement ScanStatement()
        {
            List<Statement.Var> varList = new();
            for (; ; )
            {
                Token name = Consume(IDENTIFIER, "Expected identifier");
                varList.Add(new Statement.Var(ASSIGNABLE, name, null));

                if (Match(COMMA)) { continue; }
                break;
            }

            return new Statement.Scanner(varList);
        }

        private Statement IfStatement()
        {
            Consume(LEFT_PARENTHESIS, "Expect '(' after 'if'.");
            Expression condition = Expression();
            Consume(RIGHT_PARENTHESIS, "Expect ')' after 'if'.");

            List<Statement> thenBranch = IfBlock();
            List<Statement.If> altBranches = null;
            List<Statement> elseBranch = null;

            if (Match(ELSE))
            {
                if (Check(IF))
                {
                    altBranches = new();
                    while (Match(IF))
                    {
                        altBranches.Add(new Statement.If(Expression(), IfBlock(), null, null));

                        if (Match(ELSE)) { continue; }
                    }
                    elseBranch = IfBlock();
                }
                else
                {
                    elseBranch = IfBlock();
                }
            }

            return new Statement.If(condition, thenBranch, altBranches, elseBranch);
        }

        private List<Statement> IfBlock()
        {
            List<Statement> statements = new();
            Consume(BLOCK_START, "SyntaxError: Expected IF block.");
            Consume(IF, "SyntaxError: Expected IF block");

            while (!Check(BLOCK_END) && !IsAtEndOfStream())
            {
                statements.Add(Declaration());
            }

            Consume(BLOCK_END, "SyntaxError: Unterminated IF block.");
            Consume(IF, "SyntaxError: Unterminated IF block.");

            return statements;
        }

        private Statement WhileStatement()
        {
            Consume(LEFT_PARENTHESIS, "Expect '(' after 'while'.");
            Expression condition = Expression();
            Consume(RIGHT_PARENTHESIS, "Expect ')' after condition.");
            List<Statement> body = WhileBlock();

            return new Statement.While(condition, body);
        }

        private List<Statement> WhileBlock()
        {
            List<Statement> statements = new();
            Consume(BLOCK_START, "SyntaxError: Expected WHILE block.");
            Consume(WHILE, "SyntaxError: Expected WHILE block.");

            while (!Check(BLOCK_END) && !IsAtEndOfStream())
            {
                statements.Add(Declaration());
            }

            Consume(BLOCK_END, "SyntaxError: Unterminated WHILE block.");
            Consume(WHILE, "SyntaxError: Unterminated WHILE block.");

            return statements;
        }

        private Statement VarDeclaration()
        {
            List<Statement.Var> declarations_list = new();
            Token dataType = Advance();
            for (; ; )
            {
                Token name = Consume(IDENTIFIER, "Expect variable name.");
                Expression initializer = null;
                if (Match(ASSIGNMENT)) { initializer = Expression(); }

                declarations_list.Add(new Statement.Var(dataType.TokenType, name, initializer));
                if (Match(COMMA)) { continue; }

                break;
            }
            return new Statement.VarList(declarations_list);
        }

        private Statement Declaration()
        {
            try
            {
                if (Check(VAR_INT) || Check(VAR_FLOAT) || Check(VAR_CHAR) || Check(VAR_BOOL)) return VarDeclaration();
                return Statement();
            }
            catch (StdError.ParseError)
            {
                Synchronize();
                return null;
            }
        }

        private Statement Statement()
        {
            if (Match(IF)) return IfStatement();
            if (Match(WHILE)) return WhileStatement();
            if (Match(DISPLAY))
            {
                if (Match(COLON)) return PrintStatement();
                throw StdError.ThrowParseError(Previous(), "Expected ':'");
            }
            if (Match(SCAN))
            {
                if (Match(COLON)) return ScanStatement();
                throw StdError.ThrowParseError(Previous(), "Expected ':'");
            }

            return ExpressionStatement();
        }

        private Statement ExpressionStatement()
        {
            Expression expr = Expression();
            return new Statement.Expression(expr);
        }

        private Expression Expression()
        {
            return Assignment();
        }

        private Expression Assignment()
        {
            Expression expr = OrStatement();
            if (Match(ASSIGNMENT))
            {
                Token equals = Previous();
                Expression value = Assignment();

                if (expr is Expression.Variable variable)
                {
                    Token name = variable.Name;
                    return new Expression.Assign(name, value);
                }

                StdError.ThrowParseError(equals, "Invalid assignment target.");
            }
            return expr;
        }

        private Expression OrStatement()
        {
            Expression expr = AndStatement();
            while (Match(OR))
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
            while (Match(AND))
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
            while (Match(NOT_EQUAL, EQUAL))
            {
                Token op = Previous();
                Expression right = Comparison();
                expr = new Expression.Binary(expr, op, right);
            }
            return expr;
        }

        private Expression Comparison()
        {
            Expression expr = Term();

            while (Match(GREATER, GREATER_EQUAL, LESSER, LESSER_EQUAL))
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
            while (Match(SUB, ADD, CONCAT, MOD))
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
            while (Match(DIV, MULT))
            {
                Token op = Previous();
                Expression right = Unary();
                expr = new Expression.Binary(expr, op, right);
            }
            return expr;
        }

        private Expression Unary()
        {
            if (Match(NOT, SUB))
            {
                Token op = Previous();
                Expression right = Unary();
                return new Expression.Unary(op, right);
            }
            return Primary();
        }

        private Expression Primary()
        {
            if (Match(FALSE)) return new Expression.Literal(false);
            if (Match(TRUE)) return new Expression.Literal(true);
            if (Match(NIL)) return new Expression.Literal(null);

            if (Match(VAL_INT))
            {
                return new Expression.Literal(System.Convert.ToInt32(Previous().Literal));
            }

            if (Match(VAL_FLOAT, VAL_CHAR, STRING))
            {
                return new Expression.Literal(Previous().Literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Expression.Variable(Previous());
            }

            if (Match(LEFT_PARENTHESIS))
            {
                Expression expr = Expression();
                Consume(RIGHT_PARENTHESIS, "Expect ')' after expression.");
                return new Expression.Grouping(expr);
            }

            throw StdError.ThrowParseError(LookAhead(), "Expect expression.");
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
            if (Check(type)) { return Advance(); }
            throw StdError.ThrowParseError(LookAhead(), message);
        }

        private bool Check(Token.Type type)
        {
            if (IsAtEndOfStream()) { return false; }
            return (LookAhead().TokenType == type);
        }

        private Token Advance()
        {
            if (!IsAtEndOfStream()) { _current++; }
            return Previous();
        }

        private Token LookAhead()
        {
            return _tokenStream[_current];
        }

        private Token Previous()
        {
            return _tokenStream[_current - 1];
        }

        private bool IsAtEndOfLine()
        {
            return (LookAhead().TokenType == ENDLINE);
        }

        private bool IsAtEndOfStream()
        {
            return (LookAhead().TokenType == EOF);
        }
    }
}
