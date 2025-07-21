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
        .Set("sqrt", Sqrt)
        .Set("cbrt", Cbrt)
        .Set("tan", Tan)
        .Set("atan", Atan)
        .Set("atan2", Atan2)
        .Set("tanh", Tanh)
        .Set("atanh", Atanh)
        .Set("sin", Sin)
        .Set("asin", Asin)
        .Set("sinh", Sinh)
        .Set("asinh", Asinh)
        .Set("cos", Cos)
        .Set("acos", Acos)
        .Set("cosh", Cosh)
        .Set("acosh", Acosh)
        .Set("log", Log)
        .Set("log2", Log2)
        .Set("log10", Log10)
        .Set("truncate", Truncate)
        .Set("min", Min)
        .Set("max", Max)
        .Set("clamp", Clamp)
        .Set("fma", Fma)
        .Set("avg", Avg)
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
    public static Value Sqrt(StarscriptFunctionContext ctx) => Math.Sqrt(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Cbrt(StarscriptFunctionContext ctx) => Math.Cbrt(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Tan(StarscriptFunctionContext ctx) => Math.Tan(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Atan(StarscriptFunctionContext ctx) => Math.Atan(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Atan2(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (y, x) = ctx.NextTypedPair(TypedArg.Number(1), TypedArg.Number(2));

        return Math.Atan2(y, x);
    }
    public static Value Tanh(StarscriptFunctionContext ctx) => Math.Tanh(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Atanh(StarscriptFunctionContext ctx) => Math.Atanh(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Sin(StarscriptFunctionContext ctx) => Math.Sin(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Asin(StarscriptFunctionContext ctx) => Math.Asin(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Sinh(StarscriptFunctionContext ctx) => Math.Sinh(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Asinh(StarscriptFunctionContext ctx) => Math.Asinh(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Cos(StarscriptFunctionContext ctx) => Math.Cos(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Acos(StarscriptFunctionContext ctx) => Math.Acos(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Cosh(StarscriptFunctionContext ctx) => Math.Cosh(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Acosh(StarscriptFunctionContext ctx) => Math.Acosh(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Log(StarscriptFunctionContext ctx) => Math.Log(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Log2(StarscriptFunctionContext ctx) => Math.Log2(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Log10(StarscriptFunctionContext ctx) => Math.Log10(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));
    public static Value Truncate(StarscriptFunctionContext ctx) => Math.Truncate(ctx.Constrain(Constraint.ExactCount(1)).NextNumber(1));

    public static Value Min(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (a, b) = ctx.NextTypedPair(TypedArg.Number(1), TypedArg.Number(2));

        return Math.Min(a, b);
    }
    
    public static Value Max(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(2));
        
        var (a, b) = ctx.NextTypedPair(TypedArg.Number(1), TypedArg.Number(2));
        
        return Math.Max(a, b);
    }
    
    public static Value Clamp(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(3));
        
        var (value, min, max) = ctx.NextTypedTriple(TypedArg.Number(1), TypedArg.Number(2), TypedArg.Number(3));

        return Math.Clamp(value, min, max);
    }

    public static Value Fma(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.ExactCount(3));

        var (x, y, z) = ctx.NextTypedTriple(TypedArg.Number(1), TypedArg.Number(2), TypedArg.Number(3));

        return Math.FusedMultiplyAdd(x, y, z);
    }
    
    public static Value Avg(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.AtLeast(2));

        var args = ctx.GetVariadicArguments(TypedArg.Number);

        return args.Sum() / args.Length;
    }

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