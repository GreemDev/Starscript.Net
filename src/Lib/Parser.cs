using Starscript.Internal;

namespace Starscript;

// other parts are in src/Lib/Internal/Parser

/// <summary>
/// A parser that produces an AST (Abstract Syntax Tree) from Starscript input code and reports errors. 
/// </summary>
public partial struct Parser
{
    private readonly Lexer _lexer;

    private TokenData _previous;
    private TokenData _current;

    private int _expressionDepth;

    internal Parser(string source)
    {
        _lexer = new Lexer(source);
    }

    public static Result Parse(string source) => new Parser(source).Run();
    
    public static bool TryParse(string source, out Result result)
        => !(result = new Parser(source).Run()).HasErrors;

    internal Result Run()
    {
        var result = new Result(_lexer.Source);

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
    
    public class Result : Expr.IVisitable
    {
        public Result(string source)
        {
            Exprs = [];
            Errors = [];

            Source = source;
        }

        public readonly List<Expr> Exprs;
        public readonly List<ParserError> Errors;

        public readonly string Source;

        public bool HasErrors => Errors.Count > 0;

        public void Accept(Expr.IVisitor visitor)
        {
            foreach (var expr in Exprs) 
                expr.Accept(visitor);
        } 
    }
}