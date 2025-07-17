namespace Starscript;

public abstract class ArgType<T>
{
    public string FriendlyName { get; }

    public Func<StarscriptHypervisor, string, T> Popper { get; }

    public T Pop(StarscriptHypervisor ss, string error) => Popper(ss, error);

    internal ArgType(string friendly, Func<StarscriptHypervisor, string, T> popper)
    {
        FriendlyName = friendly;
        Popper = popper;
    }
}

public static class ArgType
{
    private sealed class BooleanArgType() : ArgType<bool>("boolean (true/false)", (ss, err) => ss.PopBoolean(err));
    private sealed class StringArgType() : ArgType<string>("string", (ss, err) => ss.PopString(err));
    private sealed class NumberArgType() : ArgType<double>("number", (ss, err) => ss.PopNumber(err));
    
    public static readonly ArgType<bool> Boolean = new BooleanArgType();
    public static readonly ArgType<string> String = new StringArgType();
    public static readonly ArgType<double> Number = new NumberArgType();
}