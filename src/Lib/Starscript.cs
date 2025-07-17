using System.Diagnostics.CodeAnalysis;

namespace Starscript;

/// <summary>
///     A hypervisor capable of running compiled Starscript <see cref="Script"/>s, with contextual global variables.
/// </summary>
public partial class StarscriptHypervisor
{
    public readonly ValueMap Globals;

    public StarscriptHypervisor()
    {
        Globals = new ValueMap();
    }

    public StarscriptHypervisor(StarscriptHypervisor parent)
    {
        Globals = parent.Globals;
    }

    [DoesNotReturn]
    public static void ThrowError(string format, params object?[] args) 
        => throw new StarscriptException(args.Length == 0 ? format : string.Format(format, args));
}