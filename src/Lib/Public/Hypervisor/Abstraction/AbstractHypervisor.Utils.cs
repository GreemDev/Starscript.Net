using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Starscript.Internal;
using Starscript.Util;

namespace Starscript.Abstraction;

public partial class AbstractHypervisor<TSelf>
{
    public StringSegment Run(ExecutableScript script) => RunInternal(script, new StringBuilder());

    /// <summary>
    ///     Calls <see cref="Run(ExecutableScript)"/> after calling <see cref="ReplaceLocals(Starscript.ValueMap)"/>.
    /// </summary>
    /// <remarks>The internal locals map has been reset by the time this method returns, if persistent locals is disabled.</remarks>
    public StringSegment Run(ExecutableScript script, ValueMap locals) 
        => ReplaceLocals(locals).Run(script);

    /// <summary>
    ///     Calls <see cref="Run(ExecutableScript)"/> after calling <see cref="ReplaceLocals(Starscript.IStarscriptObject)"/>.
    /// </summary>
    /// <remarks>The internal locals map has been reset by the time this method returns, if persistent locals is disabled.</remarks>
    public StringSegment Run(ExecutableScript script, IStarscriptObject locals) 
        => ReplaceLocals(locals).Run(script);

    public static implicit operator TSelf(AbstractHypervisor<TSelf> hv) => (TSelf)hv;
    
    public static StarscriptException Error([StringSyntax("CompositeFormat")] string format, params object?[] args) 
        => new(args.Length == 0 ? format : string.Format(format, args));

#if DEBUG
    protected void DebugLog(string message,
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