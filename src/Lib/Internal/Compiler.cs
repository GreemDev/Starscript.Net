using static Starscript.Internal.Instruction;

namespace Starscript.Internal;

public class Compiler : IExprVisitor
{
    private readonly Script _output = new();

    private int _blockDepth;
    private bool _constantAppend;
    private bool _variableAppend;
    private bool _getAppend;
    private bool _callAppend;

    private Compiler()
    {
    }

    public static Script CompileFromSource(string source)
    {
        var parsed = Parser.Parse(source);
        if (parsed.HasErrors)
            throw new ParserException(parsed.Errors.First());

        return Compile(parsed);
    }
    
    public static Script Compile(ParserResult result)
    {
        var compiler = new Compiler();
        
        foreach (var expr in result.Exprs)
        {
            compiler.Compile(expr);
        }
        
        compiler._output.Write(End);

        return compiler._output;
    }

    public void Visit(Expr.Null expr) => _output.Write(Null);

    public void Visit(Expr.String expr)
        => _output.Write(_blockDepth == 0 || _constantAppend ? ConstantAppend : Constant, expr.Value);

    public void Visit(Expr.Number expr) => _output.Write(Constant, expr.Value);

    public void Visit(Expr.Boolean expr) => _output.Write(expr.Value ? True : False);

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

        if (!_constantAppend && !_variableAppend && !_getAppend && !_callAppend)
            _output.Write(Append);
        else
            _constantAppend = _variableAppend = _getAppend = _callAppend = false;

        _blockDepth--;
    }

    public void Visit(Expr.Group expr) => Compile(expr.Expr);

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
    }

    public void Visit(Expr.Variable expr)
        => _output.Write(_variableAppend ? VariableAppend : Variable, expr.Name);

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
    }

    public void Visit(Expr.Logical expr)
    {
        Compile(expr.Left);

        var endJump = _output.WriteJump(expr.Operator is Token.And ? JumpIfFalse : JumpIfTrue);
        
        _output.Write(Pop);
        Compile(expr.Right);
        
        _output.PatchJump(endJump);
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
    }

    public void Visit(Expr.Section expr)
    {
        _output.Write(Section, expr.Index);
        Compile(expr.Expr);
    }

    public void Compile(Expr? expr) => expr?.Accept(this);
}