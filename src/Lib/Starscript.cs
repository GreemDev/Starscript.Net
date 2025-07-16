namespace Starscript;

/// <summary>
///     A hypervisor capable of running compiled Starscript <see cref="Script"/>s, with contextual global variables.
/// </summary>
public partial class Starscript
{
    public readonly ValueMap Globals;

    public Starscript()
    {
        Globals = new ValueMap();
    }

    public Starscript(Starscript parent)
    {
        Globals = parent.Globals;
    }

    public void ThrowError(string format, params object?[] args) 
        => throw new StarscriptException(args.Length == 0 ? format : string.Format(format, args));
}