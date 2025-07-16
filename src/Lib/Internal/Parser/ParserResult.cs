namespace Starscript.Internal;

public struct ParserResult : IExprVisitable
{
    public ParserResult()
    {
        Exprs = [];
        Errors = [];
    }
        
    public readonly List<Expr> Exprs;
    public readonly List<ParserError> Errors;

    public bool HasErrors => Errors.Count > 0;

    public void Accept(IExprVisitor visitor)
    {
        foreach (var expr in Exprs) 
            expr.Accept(visitor);
    } 
}