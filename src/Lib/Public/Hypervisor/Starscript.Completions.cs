using Starscript.Internal;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Starscript;

public partial class StarscriptHypervisor
{
    /// <summary>
    ///     Calls the provided callback for every completion that can be resolved from global variables, and returns the parsed <paramref name="source"/>.
    /// </summary>
    /// <param name="source">Starscript input.</param>
    /// <param name="position">The position of the caret.</param>
    /// <param name="callback">What to do with each completion suggestion.</param>
    /// <param name="cancellationToken">A cancellation token you can use to short-circuit return from completion logic on a best-effort basis.</param>
    public Parser.Result ParseAndGetCompletions(string source, int position, CompletionCallback callback,
        CancellationToken cancellationToken = default)
    {
        var parserResult = Parser.Parse(source);

        foreach (var expr in parserResult.Exprs)
        {
            CompletionsExpr(source, position, expr, callback, cancellationToken);
        }

        foreach (var err in parserResult.Errors)
        {
            if (err.Expr is not null)
                CompletionsExpr(source, position, err.Expr, callback, cancellationToken);
        }

        return parserResult;
    }

    /// <summary>
    ///     Calls the provided callback for every completion that can be resolved from global variables. 
    /// </summary>
    /// <param name="source">Starscript input.</param>
    /// <param name="position">The position of the caret.</param>
    /// <param name="callback">What to do with each completion suggestion.</param>
    /// <param name="cancellationToken">A cancellation token you can use to short-circuit return from completion logic on a best-effort basis.</param>
    public void GetCompletions(string source, int position, CompletionCallback callback,
        CancellationToken cancellationToken = default)
        => _ = ParseAndGetCompletions(source, position, callback, cancellationToken);

    private void CompletionsExpr(string source, int pos, Expr expr, CompletionCallback callback,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        if (pos < expr.Start || (pos > expr.End && pos != source.Length))
            return;

        if (expr is Expr.Variable variableExpr)
        {
            var start = source[variableExpr.Start..pos];

            if (Locals != null)
            {
                foreach (var (key, value) in Locals)
                {
                    if (!key.StartsWith('_') && key.StartsWith(start))
                        callback(key, value().IsFunction);
                }
            }

            foreach (var (key, value) in Globals)
            {
                if (!key.StartsWith('_') && key.StartsWith(start))
                    callback(key, value().IsFunction);
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

                    foreach (var (subKey, subValue) in value.GetMap())
                    {
                        if (!subKey.StartsWith('_') && subKey.StartsWith(start))
                            callback(subKey, subValue().IsFunction);
                    }
                }
            }
            else
                foreach (var child in expr.Children)
                    CompletionsExpr(source, pos, child, callback, cancellationToken);
        }
        else if (expr is Expr.Block blockExpr)
        {
            if (blockExpr.Expr is null)
            {
                if (Locals != null)
                {
                    foreach (var (key, value) in Locals)
                    {
                        if (!key.StartsWith('_'))
                            callback(key, value().IsFunction);
                    }
                }

                foreach (var (key, value) in Globals)
                {
                    if (!key.StartsWith('_'))
                        callback(key, value().IsFunction);
                }
            }
            else
            {
                foreach (var child in expr.Children)
                    CompletionsExpr(source, pos, child, callback, cancellationToken);
            }
        }
        else
        {
            foreach (var child in expr.Children)
                CompletionsExpr(source, pos, child, callback, cancellationToken);
        }
    }

    private Value? Resolve(Expr expr)
    {
        if (expr is Expr.Variable variableExpr)
        {
            return GetVariable(variableExpr.Name);
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