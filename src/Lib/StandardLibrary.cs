namespace Starscript;

public static class StandardLibrary
{
    private static readonly ThreadLocal<Random> tl_Random = new(() => new Random());

    public static string TimeFormat { get; set; } = "HH:mm:ss";
    public static string DateFormat { get; set; } = "dd/MM/yyyy";

    public static void Init(StarscriptHypervisor hv)
    {
        hv.Set("PI", Math.PI);
        hv.Set("time", () => DateTime.Now.ToString(TimeFormat));
        hv.Set("date", () => DateTime.Now.ToString(DateFormat));
        hv.Set("timeUtc", () => DateTime.UtcNow.ToString(TimeFormat));
        hv.Set("dateUtc", () => DateTime.UtcNow.ToString(DateFormat));

        hv.Set("round", Round);
        hv.Set("roundToString", RoundToString);
        hv.Set("floor", Constraint.ExactCount(1), Floor);
        hv.Set("ceil", Constraint.ExactCount(1), Ceil);
        hv.Set("abs", Constraint.ExactCount(1), Abs);
        hv.Set("rand", Random);
    }

    #region Number

    public static Value Round(StarscriptFunctionContext ctx)
    {
        ctx.Constrain(Constraint.Within(1, 2));
        
        switch (ctx.ArgCount)
        {
            case 1:
                return Math.Round(ctx.NextNumber());
            case 2:
                var b = ctx.NextNumber();
                var a = ctx.NextNumber();

                var x = Math.Pow(10, b);
                return Math.Round(a * x) / x;
            default:
                ctx.Error("Unreachable.");
                throw null;
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
            var max = ctx.NextNumber();
            var min = ctx.NextNumber();

            return tl_Random.Value!.NextDouble() * (min + (max - min));
        }
        
        ctx.Error("{0} requires 0 or 2 arguments, got {1}.", ctx.FormattedName, ctx.ArgCount);
        throw null;
    }
    
    #endregion


}