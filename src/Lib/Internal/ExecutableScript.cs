namespace Starscript.Internal;

public abstract class ExecutableScript : IDisposable
{
    public bool IsDisposed { get; protected set; }
    
    public abstract ReadOnlySpan<byte> Code { get; }

    public abstract ReadOnlySpan<Value> Constants { get; }
    
    public virtual byte GetByteAt(int idx) => Code[idx];

    public int GetMaskedByteAt(int idx) => GetByteAt(idx) & 0xFF;

    public StringSegment Execute(StarscriptHypervisor hypervisor)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ExecutableScript), "Cannot execute a disposed Script.");

        return hypervisor.Run(this);
    }

    public StringSegment Execute(StarscriptHypervisor hypervisor, ValueMap locals)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ExecutableScript), "Cannot execute a disposed Script.");

        return hypervisor.Run(this, locals);
    }

    public StringSegment Execute(StarscriptHypervisor hypervisor, IStarscriptObject obj)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ExecutableScript), "Cannot execute a disposed Script.");

        return hypervisor.Run(this, obj);
    }

    public abstract void Dispose();
}