using Newtonsoft.Json;
using Silksong.PurenailUtil.Collections.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Silksong.PurenailUtil.Collections;

/// <summary>
/// Like a hash multimap, but values per key are ordered as a list rather than deduped as a set.
/// </summary>
[JsonConverter(typeof(AbstractJsonConvertibleConverter))]
public class ListMultimap<K, V> : AbstractJsonConvertible<Dictionary<K, List<V>>>, IEnumerable<(K, IReadOnlyList<V>)>
{
    private readonly Dictionary<K, List<V>> dict = [];

    internal override Dictionary<K, List<V>> ConvertToRep() => dict;

    internal override void ReadRep(Dictionary<K, List<V>> value)
    {
        foreach (var e in value) Add(e.Key, e.Value);
    }

    /// <summary>
    /// The set of distinct keys in this multimap.
    /// </summary>
    public IReadOnlyCollection<K> Keys => dict.Keys;

    /// <summary>
    /// Flattened values for this multimap.
    /// </summary>
    public IEnumerable<V> Values => dict.Values.SelectMany(v => v);

    /// <summary>
    /// Try to get the values for this key.
    /// </summary>
    public bool TryGetValues(K key, out IReadOnlyList<V> values)
    {
        if (dict.TryGetValue(key, out var set))
        {
            values = set;
            return true;
        }

        values = EmptyCollection<V>.Instance;
        return false;
    }

    /// <summary>
    /// Get the values for this key, or an empty collection if none.
    /// </summary>
    public IReadOnlyList<V> Get(K key) => dict.TryGetValue(key, out var set) ? set : EmptyCollection<V>.Instance;

    /// <summary>
    /// Clear out the multimap.
    /// </summary>
    public void Clear() => dict.Clear();

    /// <summary>
    /// Add the specified key-value pair.
    /// </summary>
    public void Add(K key, V value)
    {
        if (dict.TryGetValue(key, out var list)) list.Add(value);
        else dict.Add(key, [value]);
    }

    /// <summary>
    /// Add all the given values for this key.
    /// </summary>
    public void Add(K key, IEnumerable<V> values)
    {
        if (dict.TryGetValue(key, out var list)) list.AddRange(values);
        else dict.Add(key, [.. values]);
    }

    /// <summary>
    /// Remove all values for the given key.
    /// </summary>
    public bool Remove(K key) => dict.Remove(key);

    /// <summary>
    /// Remove the specified key-value pair.
    /// </summary>
    public bool Remove(K key, V value)
    {
        if (dict.TryGetValue(key, out var list) && list.Remove(value))
        {
            if (list.Count == 0) dict.Remove(key);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Remove the specified values for this key.
    /// </summary>
    public bool Remove(K key, IEnumerable<V> values)
    {
        if (dict.TryGetValue(key, out var list))
        {
            bool changed = false;
            foreach (var value in values) changed |= list.Remove(value);

            if (list.Count == 0) dict.Remove(key);
            return changed;
        }

        return false;
    }

    private IEnumerable<(K, IReadOnlyList<V>)> EnumeateLists() => dict.Select(e => (e.Key, (IReadOnlyList<V>)e.Value));

    public IEnumerator<(K, IReadOnlyList<V>)> GetEnumerator() => EnumeateLists().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => EnumeateLists().GetEnumerator();
}
