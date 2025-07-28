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

    private readonly bool _persistentLocals;

    public static StarscriptHypervisor Create() 
        => new();

    public static StarscriptHypervisor CreateFromParent(StarscriptHypervisor hv) 
        => new(hv.Globals);

    public static StarscriptHypervisor CreateWithLocals(ValueMap locals, bool persistentLocals = false) 
        => new(locals: locals, persistentLocals: persistentLocals);

    public static StarscriptHypervisor CreateFromParentWithLocals(StarscriptHypervisor hv, ValueMap locals, bool persistentLocals = false)
        => new(hv.Globals, locals, persistentLocals);
    
    public static StarscriptHypervisor CreateWithLocals(IStarscriptObject obj, bool persistentLocals = false)
        => CreateWithLocals(obj.ToStarscript(), persistentLocals);

    public static StarscriptHypervisor CreateFromParentWithLocals(StarscriptHypervisor hv, IStarscriptObject obj, bool persistentLocals = false)
        => CreateFromParentWithLocals(hv, obj.ToStarscript(), persistentLocals);

    private StarscriptHypervisor(ValueMap? globals = null, ValueMap? locals = null, bool persistentLocals = false)
    {
        Globals = globals?.Copy() ?? new ValueMap();
        Locals = locals;
        _persistentLocals = persistentLocals;
    }

    public static StarscriptException Error([StringSyntax("CompositeFormat")] string format, params object?[] args) 
        => new(args.Length == 0 ? format : string.Format(format, args));

    public StarscriptHypervisor ClearLocsls()
    {
        Locals = null;
        return this;
    }
    
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
    /// <remarks>The internal locals map has been reset by the time this method returns, if persistent locals is disabled.</remarks>
    public StringSegment Run(Script script, ValueMap locals) 
        => ReplaceLocals(locals).Run(script);

    /// <summary>
    ///     Calls <see cref="Run(Script)"/> after calling <see cref="ReplaceLocals(Starscript.IStarscriptObject)"/>.
    /// </summary>
    /// <remarks>The internal locals map has been reset by the time this method returns, if persistent locals is disabled.</remarks>
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