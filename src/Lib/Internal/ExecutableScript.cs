namespace Starscript.Internal;

public abstract class ExecutableScript : IDisposable
{
    public abstract ReadOnlySpan<byte> Code { get; }

    public abstract ReadOnlySpan<Value> Constants { get; }
    
    public abstract void Dispose();

    protected virtual ref readonly byte GetByteAt(int idx) => ref Code[idx];

    public ref readonly byte this[int idx] => ref GetByteAt(idx);

    public bool IsDisposed { get; protected set; }

    public int GetMasked(int idx) => GetByteAt(idx) & 0xFF;

    public StringSegment Execute(StarscriptHypervisor hypervisor) 
        => hypervisor.Run(this);

    public StringSegment Execute(StarscriptHypervisor hypervisor, ValueMap locals) 
        => hypervisor.Run(this, locals);

    public StringSegment Execute(StarscriptHypervisor hypervisor, IStarscriptObject obj) 
        => hypervisor.Run(this, obj);
}