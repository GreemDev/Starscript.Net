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

    private static double SingleNumber(StarscriptFunctionContext ctx) =>
        ctx.Constrain(Constraint.ExactlyOneArgument).NextNumber(1);

    private static (double, double) TwoNumbers(StarscriptFunctionContext ctx) =>
        ctx.Constrain(Constraint.ExactlyTwoArguments)
            .NextTypedPair(TypedArg.Number(1), TypedArg.Number(2));
    
    private static (double, double, double) ThreeNumbers(StarscriptFunctionContext ctx) =>
        ctx.Constrain(Constraint.ExactlyThreeArguments)
            .NextTypedTriple(TypedArg.Number(1), TypedArg.Number(2), TypedArg.Number(3));
    
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
    public static Value Floor(StarscriptFunctionContext ctx) => Math.Floor(SingleNumber(ctx));
    public static Value Ceil(StarscriptFunctionContext ctx) => Math.Ceiling(SingleNumber(ctx));
    public static Value Abs(StarscriptFunctionContext ctx) => Math.Abs(SingleNumber(ctx));
    public static Value Sqrt(StarscriptFunctionContext ctx) => Math.Sqrt(SingleNumber(ctx));
    public static Value Cbrt(StarscriptFunctionContext ctx) => Math.Cbrt(SingleNumber(ctx));
    public static Value Tan(StarscriptFunctionContext ctx) => Math.Tan(SingleNumber(ctx));
    public static Value Atan(StarscriptFunctionContext ctx) => Math.Atan(SingleNumber(ctx));
    public static Value Atan2(StarscriptFunctionContext ctx)
    {
        var (y, x) = TwoNumbers(ctx);

        return Math.Atan2(y, x);
    }
    public static Value Tanh(StarscriptFunctionContext ctx) => Math.Tanh(SingleNumber(ctx));
    public static Value Atanh(StarscriptFunctionContext ctx) => Math.Atanh(SingleNumber(ctx));
    public static Value Sin(StarscriptFunctionContext ctx) => Math.Sin(SingleNumber(ctx));
    public static Value Asin(StarscriptFunctionContext ctx) => Math.Asin(SingleNumber(ctx));
    public static Value Sinh(StarscriptFunctionContext ctx) => Math.Sinh(SingleNumber(ctx));
    public static Value Asinh(StarscriptFunctionContext ctx) => Math.Asinh(SingleNumber(ctx));
    public static Value Cos(StarscriptFunctionContext ctx) => Math.Cos(SingleNumber(ctx));
    public static Value Acos(StarscriptFunctionContext ctx) => Math.Acos(SingleNumber(ctx));
    public static Value Cosh(StarscriptFunctionContext ctx) => Math.Cosh(SingleNumber(ctx));
    public static Value Acosh(StarscriptFunctionContext ctx) => Math.Acosh(SingleNumber(ctx));
    public static Value Log(StarscriptFunctionContext ctx) => Math.Log(SingleNumber(ctx));
    public static Value Log2(StarscriptFunctionContext ctx) => Math.Log2(SingleNumber(ctx));
    public static Value Log10(StarscriptFunctionContext ctx) => Math.Log10(SingleNumber(ctx));
    public static Value Truncate(StarscriptFunctionContext ctx) => Math.Truncate(SingleNumber(ctx));

    public static Value Min(StarscriptFunctionContext ctx)
    {
        var (a, b) = TwoNumbers(ctx);

        return Math.Min(a, b);
    }
    
    public static Value Max(StarscriptFunctionContext ctx)
    {
        var (a, b) = TwoNumbers(ctx);
        
        return Math.Max(a, b);
    }
    
    public static Value Clamp(StarscriptFunctionContext ctx)
    {
        var (value, min, max) = ThreeNumbers(ctx);

        return Math.Clamp(value, min, max);
    }

    public static Value Fma(StarscriptFunctionContext ctx)
    {
        var (x, y, z) = ThreeNumbers(ctx);

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