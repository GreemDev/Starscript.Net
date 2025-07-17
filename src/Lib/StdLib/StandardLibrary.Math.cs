namespace Starscript;

public static partial class StandardLibrary
{
    public static StarscriptHypervisor WithStandardLibraryMath(this StarscriptHypervisor hv) => hv
        .Set("PI", Math.PI)
        .Set("E", Math.E)
        .Set("tau", Math.Tau)
        .Set("round", Round)
        .Set("roundToString", RoundToString)
        .Set("floor", Constraint.ExactCount(1), Floor)
        .Set("abs", Constraint.ExactCount(1), Abs)
        .Set("rand", Random);
    
    public static Value Round(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.Within(1, 2));
        
        switch (ctx.ArgCount)
        {
            case 1:
                return Math.Round(ctx.NextNumber());
            case 2:
                var (a, b) = ctx.NextTypedPair(ArgType.Number, ArgType.Number);

                var x = Math.Pow(10, b);
                return Math.Round(a * x) / x;
            default:
                throw ctx.Error("Unreachable.");
        }
    }

    public static Value RoundToString(StarscriptFunctionContext ctx) => Round(ctx).ToString();

    public static Value Floor(StarscriptFunctionContext ctx) => Math.Floor(ctx.NextNumber());
    
    public static Value Ceil(StarscriptFunctionContext ctx) => Math.Ceiling(ctx.NextNumber());

    public static Value Abs(StarscriptFunctionContext ctx) => Math.Abs(ctx.NextNumber());

    public static Value Random(StarscriptFunctionContext ctx)
    {
        if (ctx.ArgCount is 0)
        {
            return tl_Random.Value!.NextDouble();
        } 
        if (ctx.ArgCount is 2)
        {
            var (min, max) = ctx.NextTypedPair(ArgType.Number, ArgType.Number);

            return tl_Random.Value!.NextDouble() * (min + (max - min));
        }
        
        throw ctx.Error("{0} requires 0 or 2 arguments, got {1}.", ctx.FormattedName, ctx.ArgCount);
    }
}