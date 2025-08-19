using Starscript.Internal;
using static Starscript.Internal.Instruction;

namespace Starscript;

public partial class Compiler
{
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
        DebugLog(
            $"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', ConstAppend {_constantAppend}, BlockDepth {_blockDepth}, Value '{expr.Value}'");
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

        CompileExpr(expr.Expr);

#if DEBUG
        DebugLog(
            $"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', ConstAppend {_constantAppend}, VarAppend {_variableAppend}, GetAppend {_getAppend}, CallAppend {_callAppend}, BlockDepth {_blockDepth}");
#endif

        if (!_constantAppend && !_variableAppend && !_getAppend && !_callAppend)
            _output.Write(Append);
        else
            _constantAppend = _variableAppend = _getAppend = _callAppend = false;

        _blockDepth--;
    }

    public void Visit(Expr.Group expr)
    {
        CompileExpr(expr.Expr);

#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.Binary expr)
    {
        CompileExpr(expr.Left);

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

        CompileExpr(expr.Right);

        switch (expr.Operator)
        {
            case Token.Plus: _output.Write(Add); break;
            case Token.Minus: _output.Write(Subtract); break;
            case Token.Star: _output.Write(Multiply); break;
            case Token.Slash: _output.Write(Divide); break;
            case Token.Percentage: _output.Write(Modulo); break;
            case Token.UpArrow: _output.Write(Power); break;

            case Token.EqualEqual: _output.Write(Instruction.Equals); break;
            case Token.BangEqual: _output.Write(NotEquals); break;
            case Token.Greater: _output.Write(Greater); break;
            case Token.GreaterEqual: _output.Write(GreaterEqual); break;
            case Token.Less: _output.Write(Less); break;
            case Token.LessEqual: _output.Write(LessEqual); break;
            case Token.LeftShift: _output.Write(LeftShift); break;
            case Token.RightShift: _output.Write(RightShift); break;
        }

#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Operator {Enum.GetName(expr.Operator)}");
#endif
    }

    public void Visit(Expr.Unary expr)
    {
        CompileExpr(expr.Right);
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
        if (!variableGet) CompileExpr(expr.Object);

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
        CompileExpr(expr.Callee);

        _callAppend = false;
        foreach (var arg in expr.Arguments)
            CompileExpr(arg);

        _callAppend = prevCallAppend;
        _output.Write(_callAppend ? CallAppend : Call, (byte)expr.ArgCount);

#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.Logical expr)
    {
        CompileExpr(expr.Left);

        var endJump = _output.WriteJump(expr.Operator is Token.And ? JumpIfFalse : JumpIfTrue);

        _output.Write(Pop);
        CompileExpr(expr.Right);

        _output.PatchJump(endJump);

#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}'");
#endif
    }

    public void Visit(Expr.Conditional expr)
    {
        CompileExpr(expr.Condition);

        int falseJump = _output.WriteJump(JumpIfFalse);
        _output.Write(Pop);

        CompileExpr(expr.TrueBranch);

        int endJump = _output.WriteJump(Jump);
        _output.PatchJump(falseJump);
        _output.Write(Pop);

        CompileExpr(expr.FalseBranch);

        _output.PatchJump(endJump);

#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', FalseJump {falseJump}, EndJump {endJump}");
#endif
    }

    public void Visit(Expr.Section expr)
    {
        _output.Write(Section, expr.Index);
        CompileExpr(expr.Expr);

#if DEBUG
        DebugLog($"Compiled {expr.ExprName}: '{expr.GetSource(_source)}', Index {expr.Index}");
#endif
    }
}