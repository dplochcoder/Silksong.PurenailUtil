using Newtonsoft.Json;
using Silksong.PurenailUtil.Collections.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Silksong.PurenailUtil.Collections;

/// <summary>
/// A hash set with support for duplicate elements.
/// </summary>
[JsonConverter(typeof(AbstractJsonConvertibleConverter))]
public class HashMultiset<T> : AbstractJsonConvertible<List<(T, int)>>, ICollection<T>
{
    private readonly Dictionary<T, int> elements = [];
    private int total;

    internal override List<(T, int)> ConvertToRep() => [.. elements.Select(e => (e.Key, e.Value))];

    internal override void ReadRep(List<(T, int)> value)
    {
        foreach (var (item, count) in value) Add(item, count);
    }

    public HashMultiset() { }
    public HashMultiset(HashMultiset<T> copy)
    {
        foreach (var e in copy.elements) elements[e.Key] = e.Value;
        total = copy.total;
    }
    public HashMultiset(IEnumerable<T> elements)
    {
        foreach (var element in elements) Add(element);
    }

    /// <summary>
    /// The distinct elements in this set.
    /// </summary>
    public IReadOnlyCollection<T> Distinct => elements.Keys;

    /// <summary>
    /// The distinct elements in this set along with their counts.
    /// </summary>
    public IEnumerable<(T, int)> Counts => elements.Select(e => (e.Key, e.Value));

    /// <summary>
    /// Whether this multiset is empty.
    /// </summary>
    public bool IsEmpty => elements.Count == 0;

    /// <summary>
    /// The total number of indistinct elements in this multiset.
    /// </summary>
    int ICollection<T>.Count => total;

    /// <summary>
    /// HashMultisets are writable.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Whether at least one copy of 'element' is present.
    /// </summary>
    public bool Contains(T element) => elements.ContainsKey(element);

    /// <summary>
    /// The number of instances of 'element' present.
    /// </summary>
    public int Count(T element) => elements.TryGetValue(element, out var count) ? count : 0;

    /// <summary>
    /// Remove all elements.
    /// </summary>
    public void Clear() => elements.Clear();

    /// <summary>
    /// Add multiple copies of a given element.
    /// </summary>
    public void Add(T element, int count)
    {
        if (count == 0) return;
        if (count < 0) throw new ArgumentException($"{nameof(count)}: {count}");

        elements[element] = Count(element) + count;
        total += count;
    }

    /// <summary>
    /// Add one copy of a given element.
    /// </summary>
    public void Add(T element) => Add(element, 1);

    /// <summary>
    /// Add a collection of elements.
    /// </summary>
    public void AddRange(IEnumerable<T> elements)
    {
        foreach (var element in elements) Add(element);
    }

    /// <summary>
    /// Set the exact count of the given element.
    /// </summary>
    public bool Set(T element, int count)
    {
        if (count < 0) throw new ArgumentException($"{nameof(count)}: {count}");

        var prev = Count(element);
        if (count == prev) return false;

        total += count - prev;
        if (count == 0) elements.Remove(element);
        else elements[element] = count;
        return true;
    }

    /// <summary>
    /// Remove multiple copies of a given element.
    /// </summary>
    /// <returns>The number of elements removed.</returns>
    public int Remove(T element, int count)
    {
        if (count == 0) return 0;
        if (count < 0) throw new ArgumentException($"{nameof(count)}: {count}");

        var prev = Count(element);
        if (prev == 0) return 0;
        
        if (count >= prev)
        {
            elements.Remove(element);
            total -= prev;
            return prev;
        }
        else
        {
            elements[element] = prev - count;
            total -= count;
            return count;
        }
    }

    /// <summary>
    /// Remove a single copy of the given element.
    /// </summary>
    public bool Remove(T item) => Remove(item, 1) > 0;

    /// <summary>
    /// Copy the contents, with duplicates, into the array.
    /// </summary>
    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var element in this) array[arrayIndex++] = element;
    }

    private IEnumerable<T> Enumerate()
    {
        foreach (var e in elements)
        {
            for (int i = 0; i < e.Value; i++)
            {
                yield return e.Key;
            }
        }
    }

    public IEnumerator<T> GetEnumerator() => Enumerate().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Enumerate().GetEnumerator();
}
