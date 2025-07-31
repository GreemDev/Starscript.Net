namespace Starscript;

public static partial class StandardLibrary
{
    /// <summary>
    ///     Register all variables and functions present in the Starscript <see cref="StandardLibrary"/> strings module.
    /// </summary>
    /// <param name="hv">The current <see cref="StarscriptHypervisor"/>.</param>
    /// <returns>The current <see cref="StarscriptHypervisor"/>, for chaining convenience.</returns>
    public static StarscriptHypervisor WithStandardLibraryStrings(this StarscriptHypervisor hv) => hv
        .Set("string", String)
        .Set("uppercase", ToUpper)
        .Set("lowercase", ToLower)
        .Set("contains", Contains)
        .Set("containsIgnoreCase", ContainsIgnoreCase)
        .Set("roundToString", RoundToString)
        .Set("replace", Replace)
        .Set("replaceIgnoreCase", ReplaceIgnoreCase)
        .Set("pad", Pad)
        .Set("padLeft", PadLeft)
        .Set("padRight", PadRight);
    
    public static Value String(StarscriptFunctionContext ctx) => ctx.Constrain(Constraint.ExactlyOneArgument).PopArg().ToString();

    public static Value ToUpper(StarscriptFunctionContext ctx) => ctx.Constrain(Constraint.ExactlyOneArgument).NextString(1).ToUpper();
    
    public static Value ToLower(StarscriptFunctionContext ctx) => ctx.Constrain(Constraint.ExactlyOneArgument).NextString(1).ToLower();

    public static Value Contains(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactlyTwoArguments);
        
        var (content, search) = ctx.NextTypedPair(TypedArg.String(1), TypedArg.String(2));

        return content.Contains(search);
    }
    
    public static Value ContainsIgnoreCase(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactlyTwoArguments);
        
        var (content, search) = ctx.NextTypedPair(TypedArg.String(1), TypedArg.String(2));

        return content.Contains(search, StringComparison.OrdinalIgnoreCase);
    }
    
    public static Value Replace(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactlyThreeArguments);
        
        var (content, search, replacement) = ctx.NextTypedTriple(TypedArg.String(1), TypedArg.String(2), TypedArg.String(3));

        return content.Replace(search, replacement);
    }
    
    public static Value ReplaceIgnoreCase(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactlyThreeArguments);
        
        var (content, search, replacement) = ctx.NextTypedTriple(TypedArg.String(1), TypedArg.String(2), TypedArg.String(3));

        return content.Replace(search, replacement, StringComparison.OrdinalIgnoreCase);
    }
    
    public static Value Pad(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactlyTwoArguments);
        
        var (content, amount) = ctx.NextTypedPair(TypedArg.String(1), TypedArg.Number(2));

        var padAmount = Math.Abs((int)amount);

        return amount < 0
            ? content.PadRight(padAmount)
            : content.PadLeft(padAmount);
    }
    
    public static Value PadLeft(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactlyTwoArguments);
        
        var (content, amount) = ctx.NextTypedPair(TypedArg.String(1), TypedArg.Number(2));

        return amount < 0 
            ? throw ctx.Error($"Negative pad amount passed to {ctx.FormattedName}") 
            : content.PadLeft((int)amount);
    }
    
    public static Value PadRight(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactlyTwoArguments);
        
        var (content, amount) = ctx.NextTypedPair(TypedArg.String(1), TypedArg.Number(2));

        return amount < 0 
            ? throw ctx.Error($"Negative pad amount passed to {ctx.FormattedName}") 
            : content.PadRight((int)amount);
    }
}