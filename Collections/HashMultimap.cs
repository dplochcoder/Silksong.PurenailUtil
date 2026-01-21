using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Silksong.PurenailUtil.Collections;

/// <summary>
/// A dictionary storing multiple values at each key/
/// </summary>
public class HashMultimap<K, V> : IEnumerable<(K, IReadOnlyCollection<V>)>
{
    private readonly Dictionary<K, HashSet<V>> dict = [];

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
    public bool TryGetValues(K key, out IReadOnlyCollection<V> values)
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
    public IReadOnlyCollection<V> Get(K key) => dict.TryGetValue(key, out var set) ? set : EmptyCollection<V>.Instance;

    /// <summary>
    /// Clear out the multimap.
    /// </summary>
    public void Clear() => dict.Clear();

    /// <summary>
    /// Add the specified key-value pair.
    /// </summary>
    public bool Add(K key, V value)
    {
        if (dict.TryGetValue(key, out var set)) return set.Add(value);
        else
        {
            dict.Add(key, [value]);
            return true;
        }
    }

    /// <summary>
    /// Add all the given values for this key.
    /// </summary>
    public bool Add(K key, IEnumerable<V> values)
    {
        if (dict.TryGetValue(key, out var set))
        {
            bool changed = false;
            foreach (var value in values) changed |= set.Add(value);
            return changed;
        }

        set = [.. values];
        if (set.Count == 0) return false;

        dict[key] = set;
        return true;
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
        if (dict.TryGetValue(key, out var set) && set.Remove(value))
        {
            if (set.Count == 0) dict.Remove(key);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Remove the specified values for this key.
    /// </summary>
    public bool Remove(K key, IEnumerable<V> values)
    {
        if (dict.TryGetValue(key, out var set))
        {
            bool changed = false;
            foreach (var value in values) changed |= set.Remove(value);

            if (set.Count == 0) dict.Remove(key);
            return changed;
        }

        return false;
    }

    private IEnumerable<(K, IReadOnlyCollection<V>)> EnumeateSets() => dict.Select(e => (e.Key, (IReadOnlyCollection<V>)e.Value));

    public IEnumerator<(K, IReadOnlyCollection<V>)> GetEnumerator() => EnumeateSets().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => EnumeateSets().GetEnumerator();
}
