using System.Diagnostics.CodeAnalysis;

namespace Starscript.Abstraction;

public partial class AbstractHypervisor<TSelf>
{
    /// <summary>
    ///     Sets a variable supplier for the provided name.
    /// </summary>
    public TSelf Set(string name, Func<Value> supplier) 
    {
        Globals.Set(name, supplier);
        return this;
    }

    /// <summary>
    ///     Sets a variable supplier that always returns the same value for the provided name.
    /// </summary>
    public TSelf Set(string name, Value value) 
    {
        Globals.Set(name, value);
        return this;
    }

    /// <summary>
    ///     Sets a variable supplier that always returns the same <see cref="IStarscriptObject"/> for the provided name.
    /// </summary>
    public TSelf Set(string name, IStarscriptObject value)
    {
        Globals.Set(name, value);
        return this;
    }

    /// <summary>
    ///     Sets a function variable supplier that always returns the same value for the provided name.
    /// </summary>
    public TSelf Set(string name, StarscriptFunction function) 
    {
        Globals.Set(name, function);
        return this;
    }

    public TSelf Set(string name, Constraint constraint, ContextualStarscriptFunction contextualFunction)
    {
        Globals.Set(name, constraint, contextualFunction);
        return this;
    }

    public TSelf Set(string name, ContextualStarscriptFunction contextualFunction)
    {
        Globals.Set(name, contextualFunction);
        return this;
    }

    public TSelf NewSubMap(string name, Action<ValueMap> init)
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
    public TSelf ReplaceGlobals(ValueMap map)
    {
        Globals = map;
        return this;
    }

    /// <summary>
    ///     Sets an object variable supplier that always returns the same value for the provided name.
    /// </summary>
    public TSelf Set(string name, object obj) 
    {
        Globals.Set(name, TopLevelFunctions.Object(obj));
        return this;
    }

    /// <summary>
    ///     Removes a single value with the specified name from the globals and returns the removed value.
    /// </summary>
    public bool Remove(string name, [MaybeNullWhen(false)] out Func<Value> removedValue) 
        => Globals.Remove(name, out removedValue);

    /// <summary>
    ///     Removes all values from the globals.
    /// </summary>
    public void Clear() => Globals.Clear();
}