using Newtonsoft.Json;
using Silksong.PurenailUtil.Collections.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Silksong.PurenailUtil.Collections;

/// <summary>
/// A HashTable storing multiple values at each entry.
/// </summary>
[JsonConverter(typeof(AbstractJsonConvertibleConverter))]
public class HashMultitable<K1, K2, V> : AbstractJsonConvertible<HashTable<K1, K2, HashSet<V>>>, IEnumerable<((K1, K2), IReadOnlyCollection<V>)>
{
    private readonly HashTable<K1, K2, HashSet<V>> table = [];

    internal override HashTable<K1, K2, HashSet<V>> ConvertToRep() => table;

    internal override void ReadRep(HashTable<K1, K2, HashSet<V>> value)
    {
        foreach (var (keys, values) in value) Add(keys.Item1, keys.Item2, values);
    }

    /// <summary>
    /// Remove all entries for this multi-table.
    /// </summary>
    public void Clear() => table.Clear();

    /// <summary>
    /// Try to get the values at this entry.
    /// </summary>
    public bool TryGetValues(K1 key1, K2 key2, out IReadOnlyCollection<V> values)
    {
        if (table.TryGetValue(key1, key2, out var set))
        {
            values = set;
            return true;
        }

        values = EmptyCollection<V>.Instance;
        return false;
    }

    /// <summary>
    /// Get the values at this entry. or an empty collection if none.
    /// </summary>
    public IReadOnlyCollection<V> Get(K1 key1, K2 key2) => TryGetValues(key1, key2, out var values) ? values : EmptyCollection<V>.Instance;

    /// <summary>
    /// Add the given value to this entry.
    /// </summary>
    public bool Add(K1 key1, K2 key2, V value)
    {
        if (table.TryGetValue(key1, key2, out var set)) return set.Add(value);
        else
        {
            table.Set(key1, key2, [value]);
            return true;
        }
    }

    /// <summary>
    /// Add the given values to this entry.
    /// </summary>
    public bool Add(K1 key1, K2 key2, IEnumerable<V> values)
    {
        if (table.TryGetValue(key1, key2, out var set))
        {
            bool changed = false;
            foreach (var value in values) changed |= set.Add(value);
            return changed;
        }

        set = [.. values];
        if (set.Count == 0) return false;

        table.Set(key1, key2, set);
        return true;
    }

    /// <summary>
    /// Remove all entries for this primary key.
    /// </summary>
    public bool Remove(K1 key) => table.Remove(key);

    /// <summary>
    /// Remove all values for this entry.
    /// </summary>
    public bool Remove(K1 key1, K2 key2) => table.Remove(key1, key2);

    /// <summary>
    /// Remove the specified value.
    /// </summary>
    public bool Remove(K1 key1, K2 key2, V value)
    {
        if (table.TryGetValue(key1, key2, out var set) && set.Remove(value))
        {
            if (set.Count == 0) table.Remove(key1, key2);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Remove all specified values for this entry.
    /// </summary>
    public bool Remove(K1 key1, K2 key2, IEnumerable<V> values)
    {
        if (table.TryGetValue(key1, key2, out var set))
        {
            bool changed = false;
            foreach (var value in values) changed |= set.Remove(value);

            if (set.Count == 0) table.Remove(key1, key2);
            return changed;
        }

        return false;
    }

    private IEnumerable<((K1, K2), IReadOnlyCollection<V>)> EnumerateEntries() => table.Select(e => (e.Item1, (IReadOnlyCollection<V>)e.Item2));

    public IEnumerator<((K1, K2), IReadOnlyCollection<V>)> GetEnumerator() => EnumerateEntries().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => EnumerateEntries().GetEnumerator();
}
