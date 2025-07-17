using Starscript.Internal;

namespace Starscript;

/// <summary>
/// A parser that produces an AST (Abstract Syntax Tree) from Starscript input code and reports errors. 
/// </summary>
public partial class Parser
{
    private readonly Lexer _lexer;

    private TokenData _previous;
    private TokenData _current;

    private int _expressionDepth;

    internal Parser(string source)
    {
        _lexer = new Lexer(source);
    }

    public static ParserResult Parse(string source) => new Parser(source).Run();

    internal ParserResult Run()
    {
        var result = new ParserResult();

        Advance();

        while (!IsAtEnd)
        {
            try
            {
                result.Exprs.Add(Statement());
            }
            catch (ParseException e)
            {
                result.Errors.Add(e.Error);
                Synchronize();
            }
        }

        return result;
    }

    #region Tokenizing logic

    private Expr Statement()
    {
        if (MatchAnyNext(Token.Section))
        {
            if (_previous.Lexeme == string.Empty) 
                throw Error("Expected section index.", null);

            int start = _previous.Start;

            int index = int.Parse(_previous.Lexeme);
            Expr expr = Expression();
            expr = new Expr.Section(start, _previous.End, index, expr);

            if (index > 255) 
                throw Error("Section index cannot be larger than 255.", expr);
            return expr;
        }

        return Expression();
    }

    private Expr Expression()
    {
        return Conditional();
    }

    private Expr Conditional()
    {
        int start = _previous.Start;
        Expr expr = And();

        if (MatchAnyNext(Token.QuestionMark))
        {
            Expr trueExpr = Statement();
            Consume(Token.Colon, "Expected ':' after first part of condition.", expr);
            Expr falseExpr = Statement();
            expr = new Expr.Conditional(start, _previous.End, expr, trueExpr, falseExpr);
        }

        return expr;
    }

    private Expr And()
    {
        Expr expr = Or();

        while (MatchAnyNext(Token.And))
        {
            int start = _previous.Start;

            Expr right = Or();
            expr = new Expr.Logical(start, _previous.End, expr, Token.And, right);
        }

        return expr;
    }

    private Expr Or()
    {
        Expr expr = Equality();

        while (MatchAnyNext(Token.Or))
        {
            int start = _previous.Start;

            Expr right = Equality();
            expr = new Expr.Logical(start, _previous.End, expr, Token.Or, right);
        }

        return expr;
    }

    private Expr Equality()
    {
        int start = _previous.Start;
        Expr expr = Comparison();

        while (MatchAnyNext(Token.EqualEqual, Token.BangEqual))
        {
            Token op = _previous.Token;
            Expr right = Comparison();
            expr = new Expr.Binary(start, _previous.End, expr, op, right);
        }

        return expr;
    }

    private Expr Comparison()
    {
        int start = _previous.Start;
        Expr expr = Term();

        while (MatchAnyNext(Token.Greater, Token.GreaterEqual, Token.Less, Token.LessEqual))
        {
            Token op = _previous.Token;
            Expr right = Term();
            expr = new Expr.Binary(start, _previous.End, expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        int start = _previous.Start;
        Expr expr = Factor();

        while (MatchAnyNext(Token.Plus, Token.Minus))
        {
            Token op = _previous.Token;
            Expr right = Factor();
            expr = new Expr.Binary(start, _previous.End, expr, op, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        int start = _previous.Start;
        Expr expr = Unary();

        while (MatchAnyNext(Token.Star, Token.Slash, Token.Percentage, Token.UpArrow))
        {
            Token op = _previous.Token;
            Expr right = Unary();
            expr = new Expr.Binary(start, _previous.End, expr, op, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (MatchAnyNext(Token.Bang, Token.Minus))
        {
            int start = _previous.Start;

            Token op = _previous.Token;
            Expr right = Unary();
            return new Expr.Unary(start, _previous.End, op, right);
        }

        return Call();
    }

    private Expr Call()
    {
        Expr expr = Primary();
        int start = _previous.Start;

        while (true)
        {
            if (MatchAnyNext(Token.LeftParen))
            {
                expr = FinishCall(expr);
            }
            else if (MatchAnyNext(Token.Dot))
            {
                if (!Check(Token.Identifier))
                {
                    expr = new Expr.Get(start, _current.End, expr, "");
                }

                ref TokenData name = ref Consume(Token.Identifier, "Expected field name after '.'.", expr);
                expr = new Expr.Get(start, _previous.End, expr, name.Lexeme);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    private Expr FinishCall(Expr callee)
    {
        List<Expr> args = new(2);

        if (!Check(Token.RightParen))
        {
            do
            {
                args.Add(Expression());
            } while (MatchAnyNext(Token.Comma));
        }

        Expr expr = new Expr.Call(callee.Start, _previous.End, callee, args);
        Consume(Token.RightParen, "Expected ')' after function arguments.", expr);
        return expr;
    }

    private Expr Primary()
    {
        if (MatchAnyNext(Token.Null)) return new Expr.Null(_previous.Start, _previous.End);
        if (MatchAnyNext(Token.String)) return new Expr.String(_previous.Start, _previous.End, _previous.Lexeme);
        if (MatchAnyNext(Token.True, Token.False)) return new Expr.Boolean(_previous.Start, _previous.End, _previous.Lexeme.Equals("true"));
        if (MatchAnyNext(Token.Number)) return new Expr.Number(_previous.Start, _previous.End, double.Parse(_previous.Lexeme));
        if (MatchAnyNext(Token.Identifier)) return new Expr.Variable(_previous.Start, _previous.End, _previous.Lexeme);

        if (MatchAnyNext(Token.LeftParen))
        {
            int start = _previous.Start;

            Expr expr = Statement();
            expr = new Expr.Group(start, _previous.End, expr);

            Consume(Token.RightParen, "Expected ')' after expression.", expr);
            return expr;
        }

        if (MatchAnyNext(Token.LeftBrace))
        {
            int start = _previous.Start;
            int prevExpressionDepth = _expressionDepth;

            _expressionDepth++;
            Expr expr;

            try
            {
                expr = Statement();
            }
            catch (ParseException e)
            {
                e.Error.Expr ??= new Expr.Block(start, _previous.End, null);

                throw;
            }

            if (prevExpressionDepth == 0)
            {
                expr = new Expr.Block(start, _previous.End, expr);
            }

            Consume(Token.RightBrace, "Expected '}' after expression.", expr);
            _expressionDepth--;
            return expr;
        }

        throw Error("Expected expression.", null);
    }

    #endregion
}