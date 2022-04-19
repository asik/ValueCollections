using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ValueCollections;
public class Set<T> :
    IEquatable<Set<T>>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IImmutableSet<T>
{
    readonly ImmutableHashSet<T> _set;

    internal Set(IEnumerable<T> items, IEqualityComparer<T>? comparer = null) =>
        _set = ImmutableHashSet.CreateRange(comparer, items);

    internal Set(ImmutableHashSet<T> set) =>
        _set = set;

    #region Mathematical set operations

    public Set<T> Intersect(IEnumerable<T> other) =>
        new(_set.Intersect(other));

    public Set<T> Union(IEnumerable<T> other) =>
        new(_set.Union(other));

    public Set<T> SymmetricExcept(IEnumerable<T> other) =>
        new(_set.SymmetricExcept(other));

    public Set<T> Except(IEnumerable<T> other) =>
        new(_set.Except(other));

    /// <summary>
    /// Computes the union of two sets. This is the set of elements that are present in either set.
    /// </summary>
    /// <returns>A new <see cref="Set{T}"/> containing the union of the two sets.</returns>
    public static Set<T> operator |(Set<T> left, Set<T> right) =>
        left.Union(right);

    /// <inheritdoc cref="Set{T}.operator|(Set{T},Set{T})"/>
    public static Set<T> operator |(Set<T> left, IEnumerable<T> right) =>
        left.Union(right);

    /// <inheritdoc cref="Set{T}.operator|(Set{T},Set{T})"/>
    public static Set<T> operator |(IEnumerable<T> left, Set<T> right) =>
        right.Union(left);

    /// <summary>
    /// Computes the intersection of two sets. This is the set of elements that are present in both sets.
    /// </summary>
    /// <returns>A new <see cref="Set{T}"/> containing the intersection of the two sets.</returns>
    public static Set<T> operator &(Set<T> left, Set<T> right) =>
        left.Intersect(right);

    /// <summary>
    /// Computes the intersection of two sets. This is the set of elements that are present in both sets.
    /// </summary>
    /// <returns>A new <see cref="Set{T}"/> containing the intersection of the two sets.</returns>
    public static Set<T> operator &(Set<T> left, IEnumerable<T> right) =>
        left.Intersect(right);

    /// <summary>
    /// Computes the intersection of two sets. This is the set of elements that are present in both sets.
    /// </summary>
    /// <returns>A new <see cref="Set{T}"/> containing the intersection of the two sets.</returns>
    public static Set<T> operator &(IEnumerable<T> left, Set<T> right) =>
        right.Intersect(left);

    /// <summary>
    /// Computes the difference between two sets. 
    /// This is the set of elements in the left set that are not present in the right set.
    /// </summary>
    /// <returns>A new <see cref="Set{T}"/> containing the difference between the two sets.</returns>
    public static Set<T> operator -(Set<T> left, Set<T> right) =>
        left.Except(right);

    /// <inheritdoc cref="Set{T}.operator-(Set{T},Set{T})"/>
    public static Set<T> operator -(Set<T> left, IEnumerable<T> right) =>
        left.Except(right);

    /// <inheritdoc cref="Set{T}.operator-(Set{T},Set{T})"/>
    public static Set<T> operator -(IEnumerable<T> left, Set<T> right) =>
        new(ImmutableHashSet.CreateRange(left).Except(right._set));

    /// <summary>
    /// Computes the symmetric difference between two sets.
    /// This is the set of elements that are present in only one of the two sets.
    /// </summary>
    /// <returns>A new <see cref="Set{T}"/> containing the symmetric difference between the two sets.</returns>
    public static Set<T> operator ^(Set<T> left, Set<T> right) =>
        left.SymmetricExcept(right);

    /// <inheritdoc cref="Set{T}.operator^(Set{T},Set{T})"/>
    public static Set<T> operator ^(Set<T> left, IEnumerable<T> right) =>
        left.SymmetricExcept(right);

    /// <inheritdoc cref="Set{T}.operator^(Set{T},Set{T})"/>
    public static Set<T> operator ^(IEnumerable<T> left, Set<T> right) =>
        right.SymmetricExcept(left);

    public static bool operator >(Set<T> left, Set<T> right) =>
        left.IsProperSupersetOf(right);

    public static bool operator >(IEnumerable<T> left, Set<T> right) =>
        ImmutableHashSet.CreateRange(left).IsProperSupersetOf(right);

    public static bool operator >(Set<T> left, IEnumerable<T> right) =>
        left.IsProperSupersetOf(right);

    public static bool operator <(Set<T> left, Set<T> right) =>
        left.IsProperSubsetOf(right);

    public static bool operator <(IEnumerable<T> left, Set<T> right) =>
        ImmutableHashSet.CreateRange(left).IsProperSubsetOf(right);

    public static bool operator <(Set<T> left, IEnumerable<T> right) =>
        left.IsProperSubsetOf(right);

    public static bool operator >=(Set<T> left, Set<T> right) =>
        left.IsSupersetOf(right);

    public static bool operator >=(IEnumerable<T> left, Set<T> right) =>
        ImmutableHashSet.CreateRange(left).IsSupersetOf(right);

    public static bool operator >=(Set<T> left, IEnumerable<T> right) =>
        left.IsSupersetOf(right);

    public static bool operator <=(Set<T> left, Set<T> right) =>
        left.IsSubsetOf(right);

    public static bool operator <=(IEnumerable<T> left, Set<T> right) =>
        ImmutableHashSet.CreateRange(left).IsSubsetOf(right);

    public static bool operator <=(Set<T> left, IEnumerable<T> right) =>
        left.IsSubsetOf(right);

    #endregion

    #region IEquatable

    public bool Equals(Set<T> other) =>
        _set.SetEquals(other._set);

    public override bool Equals(object obj) =>
        obj is Set<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        foreach (var item in _set)
        {
            hashCode.Add(item);
        }
        return hashCode.ToHashCode();
    }

    public static bool operator ==(Set<T> left, Set<T> right) =>
        left.Equals(right);

    public static bool operator !=(Set<T> left, Set<T> right) =>
        !left.Equals(right);

    #endregion

    #region IReadOnlyCollection

    public int Count =>
        _set.Count;

    public ImmutableHashSet<T>.Enumerator GetEnumerator() =>
        _set.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        _set.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        _set.GetEnumerator();

    #endregion

    #region IImmutableSet

    IImmutableSet<T> IImmutableSet<T>.Clear() =>
        _set.Clear();

    public bool Contains(T value) =>
        _set.Contains(value);

    IImmutableSet<T> IImmutableSet<T>.Add(T value) =>
        _set.Add(value);

    IImmutableSet<T> IImmutableSet<T>.Remove(T value) =>
        _set.Remove(value);

    public bool TryGetValue(T equalValue, out T actualValue) =>
        _set.TryGetValue(equalValue, out actualValue);

    IImmutableSet<T> IImmutableSet<T>.Intersect(IEnumerable<T> other) =>
        _set.Intersect(other);

    IImmutableSet<T> IImmutableSet<T>.Except(IEnumerable<T> other) =>
        _set.Except(other);

    IImmutableSet<T> IImmutableSet<T>.SymmetricExcept(IEnumerable<T> other) =>
        _set.SymmetricExcept(other);

    IImmutableSet<T> IImmutableSet<T>.Union(IEnumerable<T> other) =>
        _set.Union(other);

    public bool SetEquals(IEnumerable<T> other) =>
        _set.SetEquals(other);

    public bool IsProperSubsetOf(IEnumerable<T> other) =>
        _set.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<T> other) =>
        _set.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<T> other) =>
        _set.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<T> other) =>
        _set.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<T> other) =>
        _set.Overlaps(other);

    #endregion
}

public class Set
{
    public static Set<T> Create<T>(params T[] items) =>
        new(items);
}
