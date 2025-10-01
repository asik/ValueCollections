using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ValueCollections;

// Experimenting with building another type. Nothing published here.

public partial class Map<TKey, TValue> :
    IImmutableDictionary<TKey, TValue>,
    IEquatable<Map<TKey, TValue>>
    where TKey : notnull
{
    readonly ImmutableDictionary<TKey, TValue> _dict;

    public Map(ImmutableDictionary<TKey, TValue> dict) =>
        _dict = dict;

    public static Map<TKey, TValue> Empty { get; } =
        new(ImmutableDictionary<TKey, TValue>.Empty);

    #region IEquatable

    public bool Equals(Map<TKey, TValue> other) =>
        _dict.AsEnumerable().SequenceEqual(other._dict);

    public override bool Equals(object obj) =>
        obj is Map<TKey, TValue> other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var kvp in _dict)
        {
            hashCode.Add(kvp.Key);
            hashCode.Add(kvp.Value);
        }
        return hashCode.ToHashCode();
    }

    #endregion

    #region IImmutableDictionary

    public TValue this[TKey key] =>
        _dict[key];

    public IEnumerable<TKey> Keys =>
        _dict.Keys;

    public IEnumerable<TValue> Values =>
        _dict.Values;

    public int Count =>
        _dict.Count;

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Add(TKey key, TValue value) =>
        _dict.Add(key, value);

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs) =>
        _dict.AddRange(pairs);

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Clear() =>
        _dict.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> pair) =>
        _dict.Contains(pair);

    public bool ContainsKey(TKey key) =>
        _dict.ContainsKey(key);

    public ImmutableDictionary<TKey, TValue>.Enumerator GetEnumerator() =>
        _dict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        _dict.GetEnumerator();

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
        _dict.GetEnumerator();

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Remove(TKey key) =>
        _dict.Remove(key);

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.RemoveRange(IEnumerable<TKey> keys) =>
        _dict.RemoveRange(keys);

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItem(TKey key, TValue value) =>
        _dict.SetItem(key, value);

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items) =>
        _dict.SetItems(items);

    public bool TryGetKey(TKey equalKey, out TKey actualKey) =>
        _dict.TryGetKey(equalKey, out actualKey);

    public bool TryGetValue(TKey key, out TValue value) =>
        _dict.TryGetValue(key, out value!);

    #endregion
}
