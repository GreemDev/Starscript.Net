using System.Runtime.CompilerServices;
using Starscript.Internal;
using Starscript.Util;
using static Starscript.Internal.Instruction;
// ReSharper disable MemberCanBePrivate.Global

namespace Starscript;

public partial class Compiler : Expr.IVisitor
{
    private readonly MutableScript _output;

    private int _blockDepth;
    private bool _constantAppend;
    private bool _variableAppend;
    private bool _getAppend;
    private bool _callAppend;

#if DEBUG
    private string _source = null!;
#endif

    public Compiler()
    {
        _output = new MutableScript();
    }
    
    public Compiler(ParserResult result) : this() 
        => Compile(result);

    public Compiler(
        ref MutableScript script)
    {
        _output = script;
    }

    /// <summary>
    ///     Run this <see cref="Compiler"/> on the given <see cref="ParserResult"/> imput, producing a <see cref="MutableScript"/>.
    ///     <br/>
    ///     Using <see cref="MutableScript.MoveToImmutable"/> on the result and storing its return value will allow infinite re-use of this <see cref="Compiler"/> instance.
    /// </summary>
    /// <param name="result">The <see cref="ParserResult"/> to compile.</param>
    /// <returns>The compiled <see cref="MutableScript"/>.</returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public MutableScript Compile(ParserResult result)
    {
#if DEBUG
        _source = result.Source;
#endif
        
        result.Accept(this);

        return WriteEnd();
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public MutableScript WriteEnd()
    {
        _output.Write(End);
        
#if DEBUG
        DebugLog($"Resulting mutable script size in bytes: {_output.CodeBuffer.BufferByteSize}");
#endif
        
        return _output;
    }
    
    public Script MoveToImmutableAndReset() => _output.MoveToImmutable();
    
    public void CompileExpr(Expr? expr) => expr?.Accept(this);

#if DEBUG
    
    internal static void DebugLog(string message,
        [CallerFilePath] string sourceLocation = default!,
        [CallerLineNumber] int lineNumber = default,
        [CallerMemberName] string callerName = default!)
    {
        if (DebugLogger.CompilerOutput)
            // ReSharper disable ExplicitCallerInfoArgument
            DebugLogger.Print(DebugLogSource.Compiler, message, InvocationInfo.Here(sourceLocation, lineNumber, callerName));
    }
    
#endif

}