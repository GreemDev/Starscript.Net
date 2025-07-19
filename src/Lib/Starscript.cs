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
    public static StarscriptHypervisor CreateFromParentStandalone(StarscriptHypervisor hv) => new(hv.Globals);
    public static StarscriptHypervisor CreateFromParentCopyStandalone(StarscriptHypervisor hv) 
        => new(hv.Globals.Copy());
    
    public static StarscriptHypervisor CreateWithStdLib(bool @unsafe = false) 
        => CreateStandalone().WithStandardLibrary(@unsafe);
    
    public static StarscriptHypervisor CreateFromParentWithStdLib(StarscriptHypervisor hv, bool @unsafe = false) 
        => CreateFromParentStandalone(hv).WithStandardLibrary(@unsafe);
    
    public static StarscriptHypervisor CreateFromParentCopyWithStdLib(StarscriptHypervisor hv, bool @unsafe = false) 
        => CreateFromParentCopyStandalone(hv).WithStandardLibrary(@unsafe);

    private StarscriptHypervisor(ValueMap globals)
    {
        Globals = globals.Copy();
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