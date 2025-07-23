using System.Diagnostics.CodeAnalysis;

namespace Starscript;

public partial class StarscriptHypervisor
{
    /// <summary>
    ///     Sets a variable supplier for the provided name.
    /// </summary>
    public StarscriptHypervisor Set(string name, Func<Value> supplier) 
    {
        Globals.Set(name, supplier);
        return this;
    }

    /// <summary>
    ///     Sets a variable supplier that always returns the same value for the provided name.
    /// </summary>
    public StarscriptHypervisor Set(string name, Value value) 
    {
        Globals.Set(name, value);
        return this;
    }

    /// <summary>
    ///     Sets a variable supplier that always returns the same <see cref="IStarscriptObject"/> for the provided name.
    /// </summary>
    public StarscriptHypervisor Set(string name, IStarscriptObject value)
    {
        Globals.Set(name, value);
        return this;
    }

    /// <summary>
    ///     Sets a function variable supplier that always returns the same value for the provided name.
    /// </summary>
    public StarscriptHypervisor Set(string name, StarscriptFunction function) 
    {
        Globals.Set(name, function);
        return this;
    }

    public StarscriptHypervisor Set(string name, Constraint constraint, ContextualStarscriptFunction contextualFunction)
    {
        Globals.Set(name, constraint, contextualFunction);
        return this;
    }

    public StarscriptHypervisor Set(string name, ContextualStarscriptFunction contextualFunction)
    {
        Globals.Set(name, contextualFunction);
        return this;
    }

    public StarscriptHypervisor NewSubMap(string name, Action<ValueMap> init)
    {
        var map = new ValueMap();
        init(map);

        return Set(name, map);
    }

    /// <summary>
    ///     Replaces this <see cref="StarscriptHypervisor"/>'s internal globals map with the provided <see cref="ValueMap"/> reference.
    /// </summary>
    /// <param name="map">A reference to the new globals map.</param>
    /// <remarks>StandardLibrary variables &amp; functions are provided via the globals map, so use one of the helpers in <see cref="StandardLibrary"/> to add them back if you want.</remarks>
    public StarscriptHypervisor ReplaceGlobals(ValueMap map)
    {
        Globals = map;
        return this;
    }

    /// <summary>
    ///     Sets an object variable supplier that always returns the same value for the provided name.
    /// </summary>
    public StarscriptHypervisor Set(string name, object obj) 
    {
        Globals.Set(name, TopLevelFunctions.Object(obj));
        return this;
    }

    /// <summary>
    ///     Removes all values from the globals.
    /// </summary>
    public void Clear() => Globals.Clear();

    /// <summary>
    ///     Removes a single value with the specified name from the globals and returns the removed value.
    /// </summary>
    public bool Remove(string name, [MaybeNullWhen(false)] out Func<Value> removedValue) 
        => Globals.Remove(name, out removedValue);

    /// <summary>
    ///     Returns a new <see cref="StarscriptHypervisor"/> with the globals inherited from this one.
    ///     Useful for maintaining multiple <see cref="StarscriptHypervisor"/>s for varied use-cases, inheriting from a single globals map with minor differences.
    /// </summary>
    public StarscriptHypervisor CopyGlobalsToNew() => CreateFromParent(this);
}