namespace Starscript;

public class StarscriptFunctionContext
{
    public int ArgPos { get; private set; } = 1;

    public StarscriptFunctionContext(string name, StarscriptHypervisor hypervisor, byte argCount)
    {
        Name = name;
        Hypervisor = hypervisor;
        ArgCount = argCount;
    }

    public string Name { get; }

    public string FormattedName => $"{Name}()";

    public StarscriptHypervisor Hypervisor { get; }
    public byte ArgCount { get; }
    
    public StarscriptException Error(string format, params object?[] args) => StarscriptHypervisor.Error(format, args);

    public StarscriptFunctionContext Constrain(Constraint constraint, string? customError = null)
    {
        if (!constraint.Test(ArgCount))
            throw new StarscriptException(customError ?? constraint.GetError(this));

        return this;
    }

    public (TLeft Left, TRight Right) NextTypedPair<TLeft, TRight>(
        ArgType<TLeft> leftType,
        ArgType<TRight> rightType
    )
    {
        var right = NextArg(rightType);
        var left = NextArg(leftType);

        return (left, right);
    }

    public (TLeft Left, TMiddle, TRight Right) NextTypedTriple<TLeft, TMiddle, TRight>(
        ArgType<TLeft> leftType,
        ArgType<TMiddle> middleType,
        ArgType<TRight> rightType
    )
    {
        var right = NextArg(rightType);
        var middle = NextArg(middleType);
        var left = NextArg(leftType);

        return (left, middle, right);
    }

    public Value PopArg() => Hypervisor.Pop();

    public T NextArg<T>(ArgType<T> type, string? customError = null)
        => type.Pop(Hypervisor,
            customError ?? $"Argument {ArgPos++} of {FormattedName} needs to be a {type.FriendlyName}.");

    public bool NextBoolean(string? customError = null) => NextArg(ArgType.Boolean, customError);
    public string NextString(string? customError = null) => NextArg(ArgType.String, customError);
    public double NextNumber(string? customError = null) => NextArg(ArgType.Number, customError);
}