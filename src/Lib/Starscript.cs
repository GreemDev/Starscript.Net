using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Starscript.Util;

namespace Starscript;

/// <summary>
///     A hypervisor capable of running compiled Starscript <see cref="Script"/>s, with contextual global variables and a secondary list of local variables, which is automatically cleared when a Starscript finishes.
/// </summary>
public partial class StarscriptHypervisor
{
    public ValueMap Globals { get; private set; }

    public ValueMap? Locals { get; private set; }

    public static StarscriptHypervisor Create() 
        => new();

    public static StarscriptHypervisor CreateFromParent(StarscriptHypervisor hv) 
        => new(hv.Globals);

    public static StarscriptHypervisor CreateWithLocals(ValueMap locals) 
        => new(locals: locals);

    public static StarscriptHypervisor CreateFromParentWithLocals(StarscriptHypervisor hv, ValueMap locals)
        => new(hv.Globals, locals);
    
    public static StarscriptHypervisor CreateWithLocals(IStarscriptObject obj)
        => new(locals: obj.ToStarscript());

    public static StarscriptHypervisor CreateFromParentWithLocals(StarscriptHypervisor hv, IStarscriptObject obj)
        => new(hv.Globals, obj.ToStarscript());

    private StarscriptHypervisor(ValueMap? globals = null, ValueMap? locals = null)
    {
        Globals = globals?.Copy() ?? new ValueMap();
        Locals = locals;
    }

    public static StarscriptException Error([StringSyntax("CompositeFormat")] string format, params object?[] args) 
        => new(args.Length == 0 ? format : string.Format(format, args));

    public StarscriptHypervisor ReplaceLocals(ValueMap locals)
    {
        Locals = locals;
        return this;
    }

    public StarscriptHypervisor ReplaceLocals(IStarscriptObject locals) 
        => ReplaceLocals(locals.ToStarscript());
    
    public StringSegment Run(Script script) => RunImpl(script, new StringBuilder());

    /// <summary>
    ///     Calls <see cref="Run(Script)"/> after calling <see cref="ReplaceLocals(Starscript.ValueMap)"/>.
    /// </summary>
    /// <remarks>The internal locals map has been reset by the time this method returns.</remarks>
    public StringSegment Run(Script script, ValueMap locals) 
        => ReplaceLocals(locals).Run(script);

    /// <summary>
    ///     Calls <see cref="Run(Script)"/> after calling <see cref="ReplaceLocals(Starscript.IStarscriptObject)"/>.
    /// </summary>
    /// <remarks>The internal locals map has been reset by the time this method returns.</remarks>
    public StringSegment Run(Script script, IStarscriptObject locals) 
        => ReplaceLocals(locals).Run(script);

#if DEBUG
    private void DebugLog(string message,
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!)
    {
        if (DebugLogger.HypervisorOutput)
            // ReSharper disable ExplicitCallerInfoArgument
            DebugLogger.Print(DebugLogSource.Hypervisor, message, InvocationInfo.Here(sourceLocation, lineNumber, callerName));
    }
#endif
}