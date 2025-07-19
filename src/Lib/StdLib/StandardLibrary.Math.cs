namespace Starscript;

public static partial class StandardLibrary
{
    /// <summary>
    ///     Register all variables and functions present in the Starscript <see cref="StandardLibrary"/> math module.
    /// </summary>
    /// <param name="hv">The current <see cref="StarscriptHypervisor"/>.</param>
    /// <returns>The current <see cref="StarscriptHypervisor"/>, for chaining convenience.</returns>
    public static StarscriptHypervisor WithStandardLibraryMath(this StarscriptHypervisor hv) => hv
        .Set("PI", Math.PI)
        .Set("E", Math.E)
        .Set("tau", Math.Tau)
        .Set("round", Round)
        .Set("roundToString", RoundToString)
        .Set("floor", Floor)
        .Set("ceil", Ceil)
        .Set("abs", Abs)
        .Set("rand", Random);
    
    public static Value Round(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.Within(1, 2));
        
        switch (ctx.ArgCount)
        {
            case 1:
                return Math.Round(ctx.NextNumber(1));
            case 2:
                var (a, b) = ctx.NextTypedPair(TypedArg.Number(1), TypedArg.Number(2));

                var x = Math.Pow(10, b);
                return Math.Round(a * x) / x;
            default:
                throw ctx.Error("Unreachable.");
        }
    }

    public static Value RoundToString(StarscriptFunctionContext ctx) => Round(ctx).ToString();

    public static Value Floor(StarscriptFunctionContext ctx) => Math.Floor(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    
    public static Value Ceil(StarscriptFunctionContext ctx) => Math.Ceiling(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));

    public static Value Abs(StarscriptFunctionContext ctx) => Math.Abs(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));

    public static Value Random(StarscriptFunctionContext ctx)
    {
        switch (ctx.ArgCount)
        {
            case 0:
                return tl_Random.Value!.NextDouble();
            case 2:
                var (min, max) = ctx.NextTypedPair(TypedArg.Number(1), TypedArg.Number(2));

                return tl_Random.Value!.NextDouble() * (min + (max - min));
            default:
                throw ctx.Error("{0} requires 0 or 2 arguments, got {1}.", ctx.FormattedName, ctx.ArgCount);
        }
    }
}