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
}