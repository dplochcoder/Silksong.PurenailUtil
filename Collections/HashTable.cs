using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Silksong.PurenailUtil.Collections;

/// <summary>
/// Two dimensional Dictionary, with easy adds, removals, and lookups.
/// </summary>
public class HashTable<K1, K2, V> : IEnumerable<((K1, K2), V)>
{
    private readonly Dictionary<K1, Dictionary<K2, V>> table = [];

    /// <summary>
    /// Get or set the value at this entry.
    /// </summary>
    public V this[K1 key1, K2 key2]
    {
        get => TryGetValue(key1, key2, out var value) ? value : throw new KeyNotFoundException($"({key1}, {key2})");
        set => Set(key1, key2, value);
    }

    /// <summary>
    /// Clear all entries.
    /// </summary>
    public void Clear() => table.Clear();

    /// <summary>
    /// Try to get the value at this entry.
    /// </summary>
    public bool TryGetValue(K1 key1, K2 key2, [MaybeNullWhen(false)] out V value)
    {
        if (table.TryGetValue(key1, out var dict) && dict.TryGetValue(key2, out value)) return true;

        value = default;
        return false;
    }

    /// <summary>
    /// Set the value at this entry.
    /// </summary>
    public void Set(K1 key1, K2 key2, V value)
    {
        if (table.TryGetValue(key1, out var dict)) dict[key2] = value;
        else 
        {
            dict = [];
            dict.Add(key2, value);
            table.Add(key1, dict);
        }
    }

    /// <summary>
    /// Remove all entries with this primary key.
    /// </summary>
    public bool Remove(K1 key) => table.Remove(key);

    /// <summary>
    /// Remove this entry.
    /// </summary>
    public bool Remove(K1 key1, K2 key2)
    {
        if (table.TryGetValue(key1, out var dict) && dict.Remove(key2))
        {
            if (dict.Count == 0) table.Remove(key1);
            return true;
        }

        return false;
    }

    private IEnumerable<((K1, K2), V)> EnumerateEntries()
    {
        foreach (var e1 in table)
            foreach (var e2 in e1.Value)
                yield return ((e1.Key, e2.Key), e2.Value);
    }

    public IEnumerator<((K1, K2), V)> GetEnumerator() => EnumerateEntries().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => EnumerateEntries().GetEnumerator();
}
