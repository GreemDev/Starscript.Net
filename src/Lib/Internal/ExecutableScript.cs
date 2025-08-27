using Starscript.Abstraction;

namespace Starscript.Internal;

public abstract class ExecutableScript : IDisposable
{
    public abstract ReadOnlySpan<byte> Code { get; }

    public abstract ReadOnlySpan<Value> Constants { get; }
    
    public abstract void Dispose();

    protected virtual ref readonly byte GetByteAt(int idx) => ref Code[idx];

    public ref readonly byte this[int idx] => ref GetByteAt(idx);

    public virtual bool IsDisposed { get; protected set; }

    public int GetMasked(int idx) => GetByteAt(idx) & 0xFF;

    public StringSegment Execute<THypervisor>(THypervisor hypervisor) 
        where THypervisor : AbstractHypervisor<THypervisor> 
        => hypervisor.Run(this);

    public StringSegment Execute<THypervisor>(THypervisor hypervisor, ValueMap locals) 
        where THypervisor : AbstractHypervisor<THypervisor> 
        => hypervisor.Run(this, locals);

    public StringSegment Execute<THypervisor>(THypervisor hypervisor, IStarscriptObject obj) 
        where THypervisor : AbstractHypervisor<THypervisor> 
        => hypervisor.Run(this, obj);
}