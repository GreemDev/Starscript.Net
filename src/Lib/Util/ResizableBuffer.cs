using System.Runtime.CompilerServices;

namespace Starscript.Util;

public class ResizableBuffer<T>
{
    private readonly int _initialCapacity;
    
    private T[] _buffer;
    
    public Span<T> Span => _buffer;

    public Memory<T> Memory => _buffer;

    public ReadOnlySpan<T> RoSpan => _buffer;

    public ReadOnlyMemory<T> RoMemory => _buffer;

    public ResizableBuffer(int initialCapacity = 8)
    {
        _initialCapacity = initialCapacity;
        _buffer = new T[initialCapacity];
        Array.Fill(_buffer, default);
    }

    /// <summary>
    ///     The amount of elements that actually make up the collection.
    /// </summary>
    public int CurrentSize { get; set; }

    /// <summary>
    ///     The amount of bytes currently consumed by the collection's elements.
    /// </summary>
    public long CurrentByteSize => CurrentSize * Unsafe.SizeOf<T>();

    /// <summary>
    ///     The amount of elements in the underlying buffer.
    /// </summary>
    public int BufferSize => _buffer.Length;

    /// <summary>
    ///     The amount of bytes currently consumed by this buffer.
    /// </summary>
    public long BufferByteSize => _buffer.Length * Unsafe.SizeOf<T>();

    /// <summary>
    ///     Write a value directly to the buffer. If the buffer is too small, it is resized by 50% (Length * 1.5).
    /// </summary>
    public void Write(T value)
    {
        GrowIfNeeded();
        WriteUnsafe(value);
    }
    
    /// <summary>
    ///     Write a value directly to the buffer without trying to grow it first.
    /// </summary>
    public void WriteUnsafe(T value) => _buffer[CurrentSize++] = value;

    public void GrowIfNeeded()
    {
        if (CurrentSize >= _buffer.Length)
        {
            // the Math.Max ensures the buffer can never get stuck at 0 length (0 * 1.5 = 0 = buffer doesn't grow)
            T[] newBuffer = new T[Math.Max((int)(_buffer.Length * 1.5), 2)];
            Array.Copy(_buffer, 0, newBuffer, 0, _buffer.Length);
            _buffer = newBuffer;
        }
    }

    /// <summary>
    ///     Resizes the current buffer to its initial capacity and sets the <see cref="CurrentSize"/> to 0.
    /// </summary>
    public void Reset()
    {
        Array.Resize(ref _buffer, _initialCapacity);
        CurrentSize = 0;
    }
    
    /// <summary>
    ///     Resizes the current buffer to 0 and sets the <see cref="CurrentSize"/> to 0.
    /// </summary>
    public void ResetAndClear()
    {
        Array.Resize(ref _buffer, 0);
        CurrentSize = 0;
    }

    /// <summary>
    ///     Resizes the current buffer to match the current size. This makes the buffer have an equal size to <see cref="CurrentSize"/> to trim off excess entries.
    /// </summary>
    public void TrimExcess() => Array.Resize(ref _buffer, CurrentSize);
}