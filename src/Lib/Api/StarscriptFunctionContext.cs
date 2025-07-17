using System.Diagnostics.CodeAnalysis;

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

    [DoesNotReturn]
    public void Error(string format, params object?[] args) => StarscriptHypervisor.ThrowError(format, args);

    public StarscriptFunctionContext Constrain(Constraint constraint, string? customError = null)
    {
        if (!constraint.Test(ArgCount))
            throw new StarscriptException(customError ?? constraint.GetError(this));

        return this;
    }

    public T NextArg<T>(ArgType<T> type, string? customError = null)
        => type.Pop(Hypervisor,
            customError ?? $"Argument {ArgPos++} of {FormattedName} needs to be a {type.FriendlyName}.");

    public bool NextBoolean(string? customError = null) => NextArg(ArgType.Boolean, customError);
    public string NextString(string? customError = null) => NextArg(ArgType.String, customError);
    public double NextNumber(string? customError = null) => NextArg(ArgType.Number, customError);
}