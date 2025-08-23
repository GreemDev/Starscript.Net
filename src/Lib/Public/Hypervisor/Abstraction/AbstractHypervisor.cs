namespace Starscript.Abstraction;

/// <summary>
///     A hypervisor abstraction capable of running compiled Starscript <see cref="Starscript.Internal.ExecutableScript"/>s,
///     with contextual global variables and a secondary list of local variables, which is automatically cleared when a Starscript finishes.
/// </summary>
public abstract partial class AbstractHypervisor<TSelf> where TSelf : AbstractHypervisor<TSelf>
{
    private ValueMap _globals;
    private ValueMap? _locals;

    public virtual ValueMap Globals
    {
        get => _globals;
        protected set => _globals = value ?? throw new NullReferenceException();
    }

    public virtual ValueMap? Locals
    {
        get => _locals;
        protected set => _locals = value;
    }

    protected bool PersistentLocals;

    protected AbstractHypervisor(ValueMap? globals = null, ValueMap? locals = null, bool persistentLocals = false)
    {
        _globals = globals?.Copy() ?? new ValueMap();
        _locals = locals;
        PersistentLocals = persistentLocals;
    }

    protected AbstractHypervisor() 
        => _globals = new ValueMap();

    protected Func<Value>? ResolveVariable(string name) => Locals?.GetRaw(name) ?? Globals.GetRaw(name);

    protected Value? GetVariable(string name) => ResolveVariable(name)?.Invoke();
}