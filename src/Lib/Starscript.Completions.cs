using Starscript.Internal;

namespace Starscript;

public partial class StarscriptHypervisor
{ 
    /// <summary>
    ///     Calls the provided callback for every completion that can be resolved from global variables. 
    /// </summary>
    /// <param name="source">Starscript input.</param>
    /// <param name="position">The position of the caret.</param>
    /// <param name="callback">What to do with each completion suggestion.</param>
    public void GetCompletions(string source, int position, CompletionCallback callback)
    {
        var parserResult = Parser.Parse(source);

        foreach (var expr in parserResult.Exprs)
        {
            CompletionsExpr(source, position, expr, callback);
        }

        foreach (var err in parserResult.Errors)
        {
            if (err.Expr is not null)
                CompletionsExpr(source, position, err.Expr, callback);
        }
    }
    
    private void CompletionsExpr(string source, int pos, Expr expr, CompletionCallback callback)
    {
        if (pos < expr.Start || (pos > expr.End && pos != source.Length)) 
            return;

        if (expr is Expr.Variable variableExpr)
        {
            var start = source[variableExpr.Start..pos];

            foreach (var key in Globals.Keys)
            {
                if (!key.StartsWith('_') && key.StartsWith(start))
                    callback(key, Globals.GetRaw(key)!().IsFunction);
            }
        } 
        else if (expr is Expr.Get getExpr)
        {
            if (pos >= getExpr.End - getExpr.Name.Length)
            {
                var value = Resolve(getExpr.Object);

                if (value is not null && value.IsMap)
                {
                    var start = source[(getExpr.Object.End + 1)..pos];

                    foreach (var key in value.GetMap().Keys)
                    {
                        if (!key.StartsWith('_') && key.StartsWith(start))
                            callback(key, value.GetMap().GetRaw(key)!().IsFunction);
                    }
                }
                
            }
            else
                foreach (var child in expr.Children) 
                    CompletionsExpr(source, pos, child, callback);
        }
        else if (expr is Expr.Block blockExpr)
        {
            if (blockExpr.Expr is null)
            {
                foreach (var key in Globals.Keys)
                {
                    if (!key.StartsWith('_'))
                        callback(key, Globals.GetRaw(key)!().IsFunction);
                }
            }
            else
            {
                foreach (var child in expr.Children) 
                    CompletionsExpr(source, pos, child, callback);
            }
        }
        else
        {
            foreach (var child in expr.Children) 
                CompletionsExpr(source, pos, child, callback);
        }
    }
    
    private Value? Resolve(Expr expr)
    {
        if (expr is Expr.Variable variableExpr)
        {
            return Globals.GetRaw(variableExpr.Name)?.Invoke();
        } 
        
        if (expr is Expr.Get getExpr)
        {
            var value = Resolve(getExpr.Object);
            if (value is null || !value.IsMap)
                return null;
            
            return value.GetMap().GetRaw(getExpr.Name)?.Invoke();
        }

        return null;
    }
}