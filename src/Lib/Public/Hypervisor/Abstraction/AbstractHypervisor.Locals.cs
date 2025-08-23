namespace Starscript.Abstraction;

public abstract partial class AbstractHypervisor<TSelf>
{
    public virtual TSelf ClearLocals()
    {
        if (!PersistentLocals)
            Locals = null;

        return this;
    }
    
    public TSelf ReplaceLocals(ValueMap locals)
    {
        Locals = locals;
        return this;
    }

    public TSelf ReplaceLocals(IStarscriptObject locals) 
        => ReplaceLocals(locals.ToStarscript());
}