using System.Runtime.CompilerServices;
using Starscript.Util;

namespace Starscript;

/// <summary>
///     A hypervisor capable of running compiled Starscript <see cref="Script"/>s, with contextual global variables.
/// </summary>
public partial class StarscriptHypervisor
{
    public readonly ValueMap Globals;

    public static StarscriptHypervisor CreateStandalone() => new(new ValueMap());
    
    public static StarscriptHypervisor CreateWithStdLib() 
        => new StarscriptHypervisor(new ValueMap()).WithStandardLibrary();

    public static StarscriptHypervisor CreateFromParent(StarscriptHypervisor hv) => new(hv.Globals);
    public static StarscriptHypervisor CreateFromParentWithStdLib(StarscriptHypervisor hv) 
        => new StarscriptHypervisor(hv.Globals).WithStandardLibrary();

    private StarscriptHypervisor(ValueMap globals)
    {
        Globals = globals;
    }
    
    public static StarscriptException Error(string format, params object?[] args) 
        => new(args.Length == 0 ? format : string.Format(format, args));
    
#if DEBUG
    private void DebugLog(string message,
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!)
    {
        if (DebugLogger.ParserOutput)
            // ReSharper disable ExplicitCallerInfoArgument
            DebugLogger.Print(DebugLogSource.Hypervisor, message, InvocationInfo.Here(sourceLocation, lineNumber, callerName));
    }
#endif
}