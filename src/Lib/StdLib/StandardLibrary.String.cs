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
    
    public static Value String(StarscriptFunctionContext ctx) => ctx.Constrain(Constraint.ExactCount(1)).PopArg().ToString();

    public static Value ToUpper(StarscriptFunctionContext ctx) => ctx.Constrain(Constraint.ExactCount(1)).NextString().ToUpper();
    
    public static Value ToLower(StarscriptFunctionContext ctx) => ctx.Constrain(Constraint.ExactCount(1)).NextString().ToLower();

    public static Value Contains(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (content, search) = ctx.NextTypedPair(ArgType.String, ArgType.String);

        return content.Contains(search);
    }
    
    public static Value ContainsIgnoreCase(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (content, search) = ctx.NextTypedPair(ArgType.String, ArgType.String);

        return content.Contains(search, StringComparison.OrdinalIgnoreCase);
    }
    
    public static Value Replace(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(3));
        
        var (content, search, replacement) = ctx.NextTypedTriple(ArgType.String, ArgType.String, ArgType.String);

        return content.Replace(search, replacement);
    }
    
    public static Value ReplaceIgnoreCase(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(3));
        
        var (content, search, replacement) = ctx.NextTypedTriple(ArgType.String, ArgType.String, ArgType.String);

        return content.Replace(search, replacement, StringComparison.OrdinalIgnoreCase);
    }
    
    public static Value Pad(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (content, amount) = ctx.NextTypedPair(ArgType.String, ArgType.Number);

        var padAmount = Math.Abs((int)amount);

        return amount < 0
            ? content.PadRight(padAmount)
            : content.PadLeft(padAmount);
    }
    
    public static Value PadLeft(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (content, amount) = ctx.NextTypedPair(ArgType.String, ArgType.Number);

        return amount < 0 
            ? throw ctx.Error($"Negative pad amount passed to {ctx.FormattedName}") 
            : content.PadLeft((int)amount);
    }
    
    public static Value PadRight(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (content, amount) = ctx.NextTypedPair(ArgType.String, ArgType.Number);

        return amount < 0 
            ? throw ctx.Error($"Negative pad amount passed to {ctx.FormattedName}") 
            : content.PadRight((int)amount);
    }
}