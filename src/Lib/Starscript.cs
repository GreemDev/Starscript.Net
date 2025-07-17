using System.Diagnostics.CodeAnalysis;

namespace Starscript;

/// <summary>
///     A hypervisor capable of running compiled Starscript <see cref="Script"/>s, with contextual global variables.
/// </summary>
public partial class StarscriptHypervisor
{
    public readonly ValueMap Globals;
    
    public StarscriptHypervisor(bool withStandardLibrary = true)
    {
        Globals = new ValueMap();

        if (withStandardLibrary)
            StandardLibrary.Init(this);
    }

    public StarscriptHypervisor(StarscriptHypervisor parent, bool withStandardLibrary = true)
    {
        Globals = parent.Globals;

        if (withStandardLibrary)
            StandardLibrary.Init(this);
    }

    [DoesNotReturn]
    public static void ThrowError(string format, params object?[] args) 
        => throw new StarscriptException(args.Length == 0 ? format : string.Format(format, args));
}