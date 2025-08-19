using Starscript.Internal;

namespace Starscript;

public class Script : ExecutableScript
{
    private byte[] _code;
    private Value[] _constants;

    public override ReadOnlySpan<byte> Code => _code;

    public override ReadOnlySpan<Value> Constants => _constants;

    public Script(byte[] codeBuffer, Value[] constants)
    {
        _code = codeBuffer;
        _constants = constants;
    }

    protected override byte GetByteAt(int idx)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot access bytecode of a disposed Script.");

        return _code[idx];
    }

    public override void Dispose()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot dispose an already disposed Script.");
        
        Array.Resize(ref _code, 0);
        Array.Resize(ref _constants, 0);

#if DEBUG
        Compiler.DebugLog("Destroyed script");
#endif

        IsDisposed = true;
        
        GC.SuppressFinalize(this);
    }
}