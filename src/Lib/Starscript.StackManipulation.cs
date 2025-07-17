using Starscript.Util;

namespace Starscript;

public partial class StarscriptHypervisor
{
    private readonly TransparentStack<Value> _stack = new();

    public void Push(Value? value) => _stack.Push(value ?? Value.Null);
    public Value Pop() => _stack.Pop();

    public (Value Left, Value Right) PopPair()
    {
        var right = Pop();
        var left = Pop();

        return (left, right);
    }
    
    public Value Peek() => _stack.Peek();
    public Value Peek(int offset) => _stack.Peek(offset);

    public bool PopBoolean(string error)
    {
        var a = Pop();
        
        if (!a.IsBool)
            ThrowError(error);

        return a.GetBool();
    }
    
    public double PopNumber(string error)
    {
        var a = Pop();
        
        if (!a.IsNumber)
            ThrowError(error);

        return a.GetNumber();
    }
    
    public string PopString(string error)
    {
        var a = Pop();
        
        if (!a.IsString)
            ThrowError(error);

        return a.GetString();
    }
    
    public object PopObject(string error)
    {
        var a = Pop();
        
        if (!a.IsObject)
            ThrowError(error);

        return a.GetObject();
    }
}