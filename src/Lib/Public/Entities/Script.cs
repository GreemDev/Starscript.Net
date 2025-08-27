using System.Diagnostics.CodeAnalysis;
using Starscript.Internal;

namespace Starscript;

public class Script : ExecutableScript
{
    private byte[]? _code;
    private Value[]? _constants;

    [MemberNotNullWhen(false, nameof(_code), nameof(_constants))]
    public override bool IsDisposed { get; protected set; }

    public override ReadOnlySpan<byte> Code => _code
                                               ?? throw new ObjectDisposedException(nameof(Script),
                                                   "Cannot access bytecode of a disposed Script.");

    public override ReadOnlySpan<Value> Constants => _constants
                                                     ?? throw new ObjectDisposedException(nameof(Script),
                                                         "Cannot access constants of a disposed Script.");

    public Script(byte[] codeBuffer, Value[] constants)
    {
        _code = codeBuffer ?? throw new NullReferenceException($"Cannot initialize a {nameof(Script)} with a null bytecode buffer.");
        _constants = constants ?? throw new NullReferenceException($"Cannot initialize a {nameof(Script)} with a null constants buffer.");
    }

    protected override ref readonly byte GetByteAt(int idx)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot access bytecode of a disposed Script.");

        return ref _code[idx];
    }

    public override void Dispose()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Script), "Cannot dispose an already disposed Script.");

        Array.Resize(ref _code, 0);
        Array.Resize(ref _constants, 0);

        _code = null;
        _constants = null;

#if DEBUG
        Compiler.DebugLog("Destroyed script");
#endif

        IsDisposed = true;

        GC.SuppressFinalize(this);
    }
}