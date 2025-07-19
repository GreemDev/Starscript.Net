namespace Starscript;

public class StarscriptFunctionContext
{
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
        TypedArgument<TLeft> leftType,
        TypedArgument<TRight> rightType
    )
    {
        var right = NextArg(rightType);
        var left = NextArg(leftType);

        return (left, right);
    }

    public (TLeft Left, TMiddle, TRight Right) NextTypedTriple<TLeft, TMiddle, TRight>(
        TypedArgument<TLeft> leftType,
        TypedArgument<TMiddle> middleType,
        TypedArgument<TRight> rightType
    )
    {
        var right = NextArg(rightType);
        var middle = NextArg(middleType);
        var left = NextArg(leftType);

        return (left, middle, right);
    }

    public Value PopArg() => Hypervisor.Pop();

    public T NextArg<T>(TypedArgument<T> type, string? customError = null)
        => type.Pop(Hypervisor,
            customError ?? $"Argument {type.ArgPos} of {FormattedName} needs to be a {type.FriendlyName}.");

    public bool NextBoolean(int argPos, string? customError = null) => NextArg(TypedArg.Boolean(argPos), customError);
    public string NextString(int argPos, string? customError = null) => NextArg(TypedArg.String(argPos), customError);
    public double NextNumber(int argPos, string? customError = null) => NextArg(TypedArg.Number(argPos), customError);
}