using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Starscript;

public class ValueMap : IReadOnlyDictionary<string, Func<Value>>
{
    private readonly Dictionary<string, Func<Value>> _entries = new();

    /// <summary>
    ///     Removes all keys and values from this <see cref="ValueMap"/>.
    /// </summary>
    public void Clear() => _entries.Clear();

    public ValueMap SetRaw(string name, Func<Value> value)
    {
        _entries[name] = value;
        return this;
    }

    public ValueMap Set(string name, Value value) => Set(name, () => value);
    
    public ValueMap Set(string name, StarscriptFunction function) => Set(name, () => function);

    public ValueMap Set(string name, Func<Value> value)
    {
        var dotIndex = name.IndexOf('.');
        if (dotIndex is -1)
            return SetRaw(name, value);
        
        // Split name based on the dot
        var name1 = name.Substring(0, dotIndex);
        var name2 = name.Substring(dotIndex + 1);

        ValueMap? map = null;
        if (_entries.TryGetValue(name1, out var get))
        {
            var val = get();
            if (val.IsMap) 
                map = val.GetMap();
        }

        if (map is null)
        {
            map = new ValueMap();
            SetRaw(name1, () => map);
        }

        map.Set(name2, value);

        return this;
    }

    /// <summary>
    /// Gets the variable supplier for the provided name. <br/><br/>
    /// 
    /// <b>Dot Notation:</b><br/>
    /// If the name is for example 'user.name' then it gets a value with the name 'user' from this map and calls .Get("name") on the second map. If 'user' is not a map then returns null.
    /// </summary>
    public Func<Value>? Get(string name)
    {
        var dotIndex = name.IndexOf('.');
        if (dotIndex is -1)
            return _entries[name];

        // Split name based on the dot
        var name1 = name.Substring(0, dotIndex);
        var name2 = name.Substring(dotIndex + 1);

        var value = _entries[name1]();

        return !value.IsMap
            ? null
            : value.GetMap().Get(name2);
    }

    public Func<Value>? GetRaw(string name) => _entries.GetValueOrDefault(name);

    /// <summary>
    /// Removes a single value with the specified name from this map and returns the removed value. <br/><br/>
    /// 
    /// <b>Dot Notation:</b><br/>
    /// If the name is for example 'user.name' then it attempts to get a value with the name 'user' from this map and calls .Remove("name") on the second map. If `user` is not a map then the last param is removed.
    /// </summary>
    public bool Remove(string name, out Func<Value> removedValue)
    {
        var dotIndex = name.IndexOf('.');
        if (dotIndex is -1)
            return _entries.Remove(name, out removedValue);

        // Split name based on the dot
        var name1 = name.Substring(0, dotIndex);
        var name2 = name.Substring(dotIndex + 1);

        var value = this[name1]();

        return !value.IsMap 
            ? _entries.Remove(name1, out removedValue)
            : value.GetMap().Remove(name2, out removedValue);
    }

    #region IReadOnlyDictionary impl

    public IEnumerator<KeyValuePair<string, Func<Value>>> GetEnumerator() => _entries.GetEnumerator();
    
    public int Count => _entries.Count;
    public bool ContainsKey(string key) => _entries.ContainsKey(key);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Func<Value> value) 
        => _entries.TryGetValue(key, out value);

    public Func<Value> this[string key] => Get(key)!;

    public IEnumerable<string> Keys => _entries.Keys;
    public IEnumerable<Func<Value>> Values => _entries.Values;
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}