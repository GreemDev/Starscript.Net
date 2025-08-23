using Starscript.Util;

namespace Starscript.Abstraction;

public partial class AbstractHypervisor<TSelf>
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

    protected void ClearStack() => _stack.Clear();

    public Value Peek() => _stack.Peek();
    public Value Peek(int offset) => _stack.Peek(offset);

    public bool PopBoolean(string error)
    {
        var a = Pop();
        
        return a.IsBool
            ? a.GetBool()
            : throw Error(error);
    }
    
    public double PopNumber(string error)
    {
        var a = Pop();
        
        return a.IsNumber
            ? a.GetNumber()
            : throw Error(error);
    }
    
    public string PopString(string error)
    {
        var a = Pop();
        
        return a.IsString
            ? a.GetString()
            : throw Error(error);
    }
    
    public object PopObject(string error)
    {
        var a = Pop();
        
        return a.IsObject
            ? a.GetObject()
            : throw Error(error);
    }
}