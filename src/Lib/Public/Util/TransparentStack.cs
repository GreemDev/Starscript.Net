namespace Starscript.Util;

/// <summary>
///     Represents a variable size last-in-first-out (LIFO) collection of instances of the same specified type.
///     <br/>
///     The justification for this type's existence as opposed to <see cref="Stack{T}"/> is the ability to view any element in the stack via <see cref="TransparentStack{T}.Peek(int)"/>, hence transparent.
/// </summary>
/// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
public class TransparentStack<T>
{
    private readonly ResizableBuffer<T> _buffer;

    public TransparentStack(int initialCapacity = 8)
    {
        _buffer = new ResizableBuffer<T>(initialCapacity);
    }

    public void Clear()
    {
        for (var idx = 0; idx < _buffer.CurrentSize; idx++)
        {
            _buffer.Span[idx] = default!;
        }
        
        _buffer.Reset();
    }

    public void Push(T value) => _buffer.Write(value);

    public T Pop()
    {
        var item = _buffer[--_buffer.CurrentSize];
        _buffer.Span[_buffer.CurrentSize] = default!;
        return item;
    }

    public T Peek() => _buffer[_buffer.CurrentSize - 1];
    
    public T Peek(int offset) => _buffer[_buffer.CurrentSize - 1 - offset];
}