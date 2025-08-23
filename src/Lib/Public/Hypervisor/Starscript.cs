using System.Text;
using Starscript.Abstraction;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Starscript;

/// <summary>
///     An implementation of <see cref="AbstractHypervisor{TSelf}"/>, capable of running compiled Starscript <see cref="Starscript.Internal.ExecutableScript"/>s,
///     with contextual global variables and a secondary list of local variables, which is automatically cleared when a Starscript finishes.
/// <br/><br/>
///     <see cref="AbstractHypervisor{TSelf}"/> implements basically everything itself and this implementation does not do anything special.
/// <br/>
///     As a result, the only parts of this class are <see cref="CopyGlobalsToNew"/>, the creation utility methods, and the actual implementations for what the instructions do.
/// </summary>
public partial class StarscriptHypervisor : AbstractHypervisor<StarscriptHypervisor>
{
    #region Initialization

    public static StarscriptHypervisor Create()
        => new();

    public static StarscriptHypervisor CreateFromParent(StarscriptHypervisor hv)
        => new(hv.Globals);

    public static StarscriptHypervisor CreateWithLocals(ValueMap locals, bool persistentLocals = false)
        => new(locals: locals, persistentLocals: persistentLocals);

    public static StarscriptHypervisor CreateFromParentWithLocals(StarscriptHypervisor hv, ValueMap locals,
        bool persistentLocals = false)
        => new(hv.Globals, locals, persistentLocals);

    public static StarscriptHypervisor CreateWithLocals(IStarscriptObject obj, bool persistentLocals = false)
        => CreateWithLocals(obj.ToStarscript(), persistentLocals);

    public static StarscriptHypervisor CreateFromParentWithLocals(StarscriptHypervisor hv, IStarscriptObject obj,
        bool persistentLocals = false)
        => CreateFromParentWithLocals(hv, obj.ToStarscript(), persistentLocals);

    // ReSharper disable once MemberCanBePrivate.Global
    protected StarscriptHypervisor(ValueMap? globals = null, ValueMap? locals = null, bool persistentLocals = false)
        : base(globals, locals, persistentLocals)
    {
    }

    #endregion
    
    /// <summary>
    ///     Returns a new <see cref="StarscriptHypervisor"/> with the globals inherited from this one.
    ///     Useful for maintaining multiple <see cref="StarscriptHypervisor"/>s for varied use-cases, inheriting from a single globals map with minor differences.
    /// </summary>
    public StarscriptHypervisor CopyGlobalsToNew() => CreateFromParent(this);
}