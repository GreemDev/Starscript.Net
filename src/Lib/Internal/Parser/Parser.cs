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
        var result = new ParserResult(_lexer.Source);

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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Section)} created: Start {start}, End {_previous.End}, " +
                     $"Index {index} ExprType {expr.ExprName}");
#endif
            
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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Conditional)} created: Start {start}, End {_previous.End}, " +
                     $"ConditionType {expr.ExprName} TrueBranchType {trueExpr.ExprName}, FalseBranchType {falseExpr.ExprName}");
#endif
            
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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Logical)} created: Start {start}, End {_previous.End}, LeftType {expr.ExprName} Operator {Enum.GetName(Token.And)}, RightType {right.ExprName}");
#endif
            
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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Logical)} created: Start {start}, End {_previous.End}, LeftType {expr.ExprName} Operator {Enum.GetName(Token.Or)}, RightType {right.ExprName}");
#endif
            
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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Binary)} created: Start {start}, End {_previous.End}, LeftType {expr.ExprName} Operator {Enum.GetName(op)}, RightType {right.ExprName}");
#endif
            
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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Binary)} created: Start {start}, End {_previous.End}, LeftType {expr.ExprName} Operator {Enum.GetName(op)}, RightType {right.ExprName}");
#endif
            
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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Binary)} created: Start {start}, End {_previous.End}, LeftType {expr.ExprName} Operator {Enum.GetName(op)}, RightType {right.ExprName}");
#endif
            
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

#if DEBUG
            DebugLog($"Expr {nameof(Expr.Binary)} created: Start {start}, End {_previous.End}, LeftType {expr.ExprName} Operator {Enum.GetName(op)}, RightType {right.ExprName}");
#endif

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
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Unary)} created: Start {start}, End {_previous.End}, Operator {Enum.GetName(op)}, RightType {right.ExprName}");
#endif
            
            return new Expr.Unary(start, _previous.End, op, right);
        }

        return Call();
    }

    private Expr Call()
    {
        Expr expr = Primary();
        int start = _previous.Start;
        
#if DEBUG
        int? end = null;
#endif
        


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
#if DEBUG
                    end = _current.End;
#endif
                }

                ref TokenData name = ref Consume(Token.Identifier, "Expected field name after '.'.", expr);
                expr = new Expr.Get(start, _previous.End, expr, name.Lexeme);
#if DEBUG
                end = _previous.End;
#endif
            }
            else
            {
                break;
            }
        }
        
#if DEBUG
        if (expr is Expr.Get getExpr)
            DebugLog($"Expr {nameof(Expr.Get)} created: Start {start}, End {end}, PrimaryType {getExpr.Object.ExprName}");
#endif

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
        
#if DEBUG
        DebugLog($"Expr {nameof(Expr.Call)} created: Start {callee.Start}, End {_previous.End}, " +
                 $"CalleeType {callee.ExprName}, ArgTypes ({string.Join(", ", args.Select(x => x.ExprName))})");
#endif
        
        return expr;
    }

    private Expr Primary()
    {
        if (MatchAnyNext(Token.Null))
        {
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Null)} created: Start {_previous.Start}, End {_previous.End}");
#endif
            
            return new Expr.Null(_previous.Start, _previous.End);
        }

        if (MatchAnyNext(Token.String))
        {
#if DEBUG
            DebugLog($"Expr {nameof(Expr.String)} created: Start {_previous.Start}, End {_previous.End}, Lexeme '{_previous.Lexeme}'");
#endif
            
            return new Expr.String(_previous.Start, _previous.End, _previous.Lexeme);
        }

        if (MatchAnyNext(Token.True, Token.False))
        {
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Boolean)} created: Start {_previous.Start}, End {_previous.End}, Lexeme '{_previous.Lexeme}', Value {_previous.Lexeme.Equals("true")}");
#endif
            
            return new Expr.Boolean(_previous.Start, _previous.End, _previous.Lexeme.Equals("true"));
        }

        if (MatchAnyNext(Token.Number))
        {
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Number)} created: Start {_previous.Start}, End {_previous.End}, " +
                     $"Lexeme '{_previous.Lexeme}', Value {double.Parse(_previous.Lexeme).ToString(Value.FullDoubleFormatInfo)}");
#endif
            
            return new Expr.Number(_previous.Start, _previous.End, double.Parse(_previous.Lexeme));
        }

        if (MatchAnyNext(Token.Identifier))
        {
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Variable)} created: Start {_previous.Start}, End {_previous.End}, Lexeme '{_previous.Lexeme}'");
#endif
            
            return new Expr.Variable(_previous.Start, _previous.End, _previous.Lexeme);
        }

        if (MatchAnyNext(Token.LeftParen))
        {
            int start = _previous.Start;

            Expr expr = Statement();
            expr = new Expr.Group(start, _previous.End, expr);

            Consume(Token.RightParen, "Expected ')' after expression.", expr);
            
#if DEBUG
            DebugLog($"Expr {nameof(Expr.Group)} created: Start {start}, End {_previous.End}, ExprType {expr.ExprName}");
#endif
            
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
            
#if DEBUG
            DebugLog($"Expr {expr.ExprName} created: Start {start}, End {_previous.End}, ExpressionDepth {_expressionDepth}, " +
                     $"ExprType {(expr is Expr.Block blockExpr ? blockExpr.Expr?.ExprName : "null")}");
#endif
            
            _expressionDepth--;
            return expr;
        }

        throw Error("Expected expression.", null);
    }

    #endregion
}