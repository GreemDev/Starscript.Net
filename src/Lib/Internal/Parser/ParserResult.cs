using Starscript.Internal;

namespace Starscript;

public class ParserResult : Expr.IVisitable
{
    public ParserResult(string source)
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