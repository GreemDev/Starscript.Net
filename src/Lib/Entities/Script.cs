namespace Starscript;

public class Script : IDisposable
{
    private bool _disposed;
    
    private byte[] _code;
    private Value[] _constants;

    public ReadOnlySpan<byte> Code => _code;

    public ReadOnlySpan<Value> Constants => _constants;

    public Script(byte[] codeBuffer, Value[] constants)
    {
        _code = codeBuffer;
        _constants = constants;
    }

    public byte GetByteAt(int idx)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot access bytecode of a disposed Script.");

        return _code[idx];
    }

    public int GetMaskedByteAt(int idx) => GetByteAt(idx) & 0xFF;

    public StringSegment Execute(StarscriptHypervisor hypervisor)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot execute a disposed Script.");

        return hypervisor.Run(this);
    }

    public StringSegment Execute(StarscriptHypervisor hypervisor, ValueMap locals)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot execute a disposed Script.");

        return hypervisor.Run(this, locals);
    }

    public StringSegment Execute(StarscriptHypervisor hypervisor, IStarscriptObject obj)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot execute a disposed Script.");

        return hypervisor.Run(this, obj);
    }

    public void Dispose()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot dispose an already disposed Script.");
        
        Array.Resize(ref _code, 0);
        Array.Resize(ref _constants, 0);

#if DEBUG
        Compiler.DebugLog("Destroyed script");
#endif

        _disposed = true;
        
        GC.SuppressFinalize(this);
    }
}