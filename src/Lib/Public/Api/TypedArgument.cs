namespace Starscript;

public abstract class TypedArgument<T>
{
    public string FriendlyName { get; }

    private readonly Func<StarscriptHypervisor, string, T> _popper;

    internal int ArgPos { get; }

    public T Pop(StarscriptHypervisor ss, string error) => _popper(ss, error);

    internal TypedArgument(string friendly, int argPos, Func<StarscriptHypervisor, string, T> popper)
    {
        FriendlyName = friendly;
        ArgPos = argPos;
        _popper = popper;
    }
}

public static class TypedArg
{
    private sealed class BooleanTypedArgument(int argPos) : TypedArgument<bool>("boolean (true/false)", argPos, (ss, err) => ss.PopBoolean(err));
    private sealed class StringTypedArgument(int argPos) : TypedArgument<string>("string", argPos, (ss, err) => ss.PopString(err));
    private sealed class NumberTypedArgument(int argPos) : TypedArgument<double>("number", argPos, (ss, err) => ss.PopNumber(err));
    
    public static TypedArgument<bool> Boolean(int argPos) => new BooleanTypedArgument(argPos);
    public static TypedArgument<string> String(int argPos) => new StringTypedArgument(argPos);
    public static TypedArgument<double> Number(int argPos) => new NumberTypedArgument(argPos);
}