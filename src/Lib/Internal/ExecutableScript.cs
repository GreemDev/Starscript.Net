namespace Starscript.Internal;

public abstract class ExecutableScript : IDisposable
{
    public abstract ReadOnlySpan<byte> Code { get; }

    public abstract ReadOnlySpan<Value> Constants { get; }
    
    public abstract void Dispose();

    public virtual byte GetByteAt(int idx) => Code[idx];


    public bool IsDisposed { get; protected set; }

    public int GetMaskedByteAt(int idx) => GetByteAt(idx) & 0xFF;

    public StringSegment Execute(StarscriptHypervisor hypervisor) 
        => hypervisor.Run(this);

    public StringSegment Execute(StarscriptHypervisor hypervisor, ValueMap locals) 
        => hypervisor.Run(this, locals);

    public StringSegment Execute(StarscriptHypervisor hypervisor, IStarscriptObject obj) 
        => hypervisor.Run(this, obj);
}