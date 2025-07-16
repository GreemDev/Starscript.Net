namespace Starscript.Util;

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
        var item = _buffer.RoSpan[--_buffer.CurrentSize];
        _buffer.Span[_buffer.CurrentSize] = default!;
        return item;
    }

    public T Peek() => _buffer.RoSpan[_buffer.CurrentSize - 1];
    
    public T Peek(int offset) => _buffer.RoSpan[_buffer.CurrentSize - 1 - offset];
}