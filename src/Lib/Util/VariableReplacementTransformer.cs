using System.Diagnostics.CodeAnalysis;
using System.Text;
using Starscript.Internal;

namespace Starscript.Util;

public class VariableReplacementTransformer : AbstractExprVisitor
{
    private readonly Dictionary<string, Func<string>> _replacers = new();
    private readonly StringBuilder _sb = new();

    public bool TryAddReplacer(string name, Func<string> replacement) => _replacers.TryAdd(name, replacement);

    public override void Visit(Expr.Variable expr) => TryReplace(expr, expr.Name);

    public override void Visit(Expr.Get expr)
    {
        if (TryGetFullName(expr, out var name)) 
            TryReplace(expr, name);
    }

    private void TryReplace(Expr expr, string name)
    {
        if (!_replacers.TryGetValue(name, out var replacer))
            return;

        var replacement = CreateReplacement(replacer());
        expr.Replace(replacement);
    }

    private Expr CreateReplacement(string replacement)
    {
        var parts = replacement.Split('.');
        if (parts.Length is 0)
            throw new FormatException("Cannot replace with an empty replacement");

        Expr expr = new Expr.Variable(0, 0, parts[0]);

        foreach (var part in parts.Skip(1))
        {
            expr = new Expr.Get(0, 0, expr, part);
        }

        return expr;
    }

    private bool TryGetFullName(Expr.Get expr, [MaybeNullWhen(false)] out string name)
    {
        try
        {
            GetFullNameInternal(expr);
        }
        catch (InvalidOperationException)
        {
            _sb.Length = 0;
            name = null;
            return false;
        }

        name = _sb.ToString();
        _sb.Length = 0;
        return true;
    }

    private void GetFullNameInternal(Expr.Get expr)
    {
        switch (expr.Object)
        {
            case Expr.Get getExpr:
                GetFullNameInternal(getExpr);
                break;
            case Expr.Variable varExpr:
                _sb.Append(varExpr.Name);
                break;
            default:
                throw new InvalidOperationException();
        }
    }
}