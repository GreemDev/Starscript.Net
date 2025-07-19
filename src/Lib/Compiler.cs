using System.Runtime.CompilerServices;
using Starscript.Internal;
using Starscript.Util;
using static Starscript.Internal.Instruction;
// ReSharper disable MemberCanBePrivate.Global

namespace Starscript;

public class Compiler : Expr.IVisitor
{
    private readonly MutableScript _output;

    private int _blockDepth;
    private bool _constantAppend;
    private bool _variableAppend;
    private bool _getAppend;
    private bool _callAppend;

#if DEBUG
    private readonly string _source;
#endif

    public Compiler(
#if DEBUG
        string source
#endif
        )
    {
#if DEBUG
        _source = source;
#endif
        _output = new MutableScript();
    }

    public Compiler(
#if DEBUG
        string source,
#endif
        ref MutableScript script)
    {
#if DEBUG
        _source = source;
#endif
        _output = script;
    }
    
    /// <exception cref="ParseException">Thrown if the internal <see cref="ParserResult"/> contains any errors.</exception>
    public static Script DirectCompile(string source)
    {
        var parsed = Parser.Parse(source);
        if (parsed.HasErrors)
            throw new ParseException(parsed.Errors.First());

        return Compile(parsed);
    }
    
    public static Script Compile(ParserResult result)
    {
        var compiler =
#if DEBUG
            new Compiler(result.Source);
#else
            new Compiler();
#endif

        var mutableScript = compiler.Run(result);
        
#if DEBUG
        DebugLog($"Resulting mutable script size in bytes before trim: '{mutableScript.CodeBuffer.BufferByteSize}'");
#endif

        var immutableScript = mutableScript.MoveToImmutable();
        
#if DEBUG
        DebugLog($"Resulting immutable script size in bytes after move to immutable: '{immutableScript.Code.Length * Unsafe.SizeOf<byte>()}'");
#endif

        return immutableScript;
    }

    public MutableScript Run(ParserResult result)
    {
        var compiler =
#if DEBUG
            new Compiler(result.Source);
#else
            new Compiler();
#endif
        
        result.Accept(compiler);

        compiler.WriteEnd();

        return compiler._output;
    }

    public void WriteEnd() => _output.Write(End);
    public Script MoveToImmutableAndReset() => _output.MoveToImmutable();

    public void Visit(Expr.Null expr)
    {
        _output.Write(Null);
        
#if DEBUG
        DebugLog($"Compiled '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.String expr)
    {
        _output.Write(_blockDepth == 0 || _constantAppend ? ConstantAppend : Constant, expr.Value);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', ConstAppend {_constantAppend}, BlockDepth {_blockDepth}, Value '{expr.Value}'");
#endif
    }

    public void Visit(Expr.Number expr)
    {
        _output.Write(Constant, expr.Value);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Value {expr.Value}");
#endif
    }

    public void Visit(Expr.Boolean expr)
    {
        _output.Write(expr.Value ? True : False);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Value: {expr.Value}");
#endif
    }

    public void Visit(Expr.Block expr)
    {
        _blockDepth++;

        switch (expr.Expr)
        {
            case Expr.String:
                _constantAppend = true;
                break;
            case Expr.Variable:
                _variableAppend = true;
                break;
            case Expr.Get:
                _getAppend = true;
                break;
            case Expr.Call:
                _callAppend = true;
                break;
        }

        Compile(expr.Expr);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', ConstAppend {_constantAppend}, VarAppend {_variableAppend}, GetAppend {_getAppend}, CallAppend {_callAppend}, BlockDepth {_blockDepth}");
#endif

        if (!_constantAppend && !_variableAppend && !_getAppend && !_callAppend)
            _output.Write(Append);
        else
            _constantAppend = _variableAppend = _getAppend = _callAppend = false;

        _blockDepth--;
    }

    public void Visit(Expr.Group expr)
    {
        Compile(expr.Expr);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.Binary expr)
    {
        Compile(expr.Left);

        if (expr.Operator is Token.Plus)
        {
            switch (expr.Right)
            {
                case Expr.String stringExpr:
                    _output.Write(AddConstant, stringExpr.Value);
                    return;
                case Expr.Number numberExpr:
                    _output.Write(AddConstant, numberExpr.Value);
                    return;
            }
        }

        Compile(expr.Right);

        switch (expr.Operator)
        {
            case Token.Plus:         _output.Write(Add); break;
            case Token.Minus:        _output.Write(Subtract); break;
            case Token.Star:         _output.Write(Multiply); break;
            case Token.Slash:        _output.Write(Divide); break;
            case Token.Percentage:   _output.Write(Modulo); break;
            case Token.UpArrow:      _output.Write(Power); break;

            case Token.EqualEqual:   _output.Write(Instruction.Equals); break;
            case Token.BangEqual:    _output.Write(NotEquals); break;
            case Token.Greater:      _output.Write(Greater); break;
            case Token.GreaterEqual: _output.Write(GreaterEqual); break;
            case Token.Less:         _output.Write(Less); break;
            case Token.LessEqual:    _output.Write(LessEqual); break;
        }
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Operator {Enum.GetName(expr.Operator)}");
#endif
    }

    public void Visit(Expr.Unary expr)
    {
        Compile(expr.Right);
        switch (expr.Operator)
        {
            case Token.Bang:
                _output.Write(Not);
                break;
            case Token.Minus:
                _output.Write(Negate);
                break;
        }
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Token {Enum.GetName(expr.Operator)}");
#endif
    }

    public void Visit(Expr.Variable expr)
    {
        _output.Write(_variableAppend ? VariableAppend : Variable, expr.Name);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Name {expr.GetSource(_source)}");
#endif
    }

    public void Visit(Expr.Get expr)
    {
        var prevGetAppend = _getAppend;
        _getAppend = false;

        var variableGet = expr.Object is Expr.Variable;
        if (!variableGet) Compile(expr.Object);

        _getAppend = prevGetAppend;

        if (variableGet)
        {
            _output.Write(_getAppend ? VariableGetAppend : VariableGet, ((Expr.Variable)expr.Object).Name);
            _output.WriteConstant(expr.Name);
        }
        else
            _output.Write(_getAppend ? GetAppend : Get, expr.Name);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.Call expr)
    {
        var prevCallAppend = _callAppend;
        Compile(expr.Callee);

        _callAppend = false;
        foreach (var arg in expr.Arguments)
            Compile(arg);

        _callAppend = prevCallAppend;
        _output.Write(_callAppend ? CallAppend : Call, (byte)expr.ArgCount);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.Logical expr)
    {
        Compile(expr.Left);

        var endJump = _output.WriteJump(expr.Operator is Token.And ? JumpIfFalse : JumpIfTrue);
        
        _output.Write(Pop);
        Compile(expr.Right);
        
        _output.PatchJump(endJump);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.Conditional expr)
    {
        Compile(expr.Condition);

        int falseJump = _output.WriteJump(JumpIfFalse);
        _output.Write(Pop);
        
        Compile(expr.TrueBranch);
        
        int endJump = _output.WriteJump(Jump);
        _output.PatchJump(falseJump);
        _output.Write(Pop);
        
        Compile(expr.FalseBranch);
        
        _output.PatchJump(endJump);
        
#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', FalseJump {falseJump}, EndJump {endJump}");
#endif
    }

    public void Visit(Expr.Section expr)
    {
        _output.Write(Section, expr.Index);
        Compile(expr.Expr);

#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Index {expr.Index}");
#endif
    }

    public void Compile(Expr? expr) => expr?.Accept(this);

#if DEBUG
    
    private static void DebugLog(string message,
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