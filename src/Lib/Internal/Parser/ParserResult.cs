namespace Starscript.Internal;

public class ParserResult : IExprVisitable
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

    public void Accept(IExprVisitor visitor)
    {
        foreach (var expr in Exprs) 
            expr.Accept(visitor);
    } 
}