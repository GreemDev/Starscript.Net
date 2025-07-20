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

    public static ParserResult Parse(string source) => new Parser(source).Run();
    
    public static bool TryParse(string source, out ParserResult result)
        => !(result = new Parser(source).Run()).HasErrors;

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
}