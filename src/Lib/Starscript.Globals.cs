namespace Starscript;

public partial class Starscript
{
    /// <summary>
    ///     Sets a variable supplier for the provided name.
    /// </summary>
    public Starscript Set(string name, Func<Value> supplier) {
        Globals.Set(name, supplier);
        return this;
    }

    /// <summary>
    ///     Sets a variable supplier that always returns the same value for the provided name.
    /// </summary>
    public Starscript Set(string name, Value value) {
        Globals.Set(name, value);
        return this;
    }

    /// <summary>
    ///     Sets a function variable supplier that always returns the same value for the provided name.
    /// </summary>
    public Starscript Set(string name, StarscriptFunction function) {
        Globals.Set(name, function);
        return this;
    }
    
    public Starscript Set(string name, Constraint constraint, ContextualStarscriptFunction contextualFunction) =>
        Set(name, (ss, argCount) 
            => contextualFunction(new StarscriptFunctionContext(name, ss, argCount).Constrain(constraint)));
    
    public Starscript Set(string name, ContextualStarscriptFunction contextualFunction) =>
        Set(name, (ss, argCount) 
            => contextualFunction(new StarscriptFunctionContext(name, ss, argCount)));
    
    public Starscript SetToString(Func<string> getter) 
        => Set("_toString", () => getter());

    public Starscript NewSubMap(string name, Action<ValueMap> init)
    {
        var map = new ValueMap();
        init(map);

        return Set(name, map);
    }

    /// <summary>
    ///     Sets an object variable supplier that always returns the same value for the provided name.
    /// </summary>
    public Starscript Set(string name, object obj) {
        Globals.Set(name, TopLevelFunctions.Object(obj));
        return this;
    }

    /// <summary>
    ///     Removes all values from the globals.
    /// </summary>
    public void Clear() => Globals.Clear();

    /** Removes a single value with the specified name from the globals and returns the removed value. <br><br> See {@link ValueMap#remove(String)} for dot notation. */
    public bool Remove(string name, out Func<Value> removedValue) => Globals.Remove(name, out removedValue);
}