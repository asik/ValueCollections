using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ValueCollections;

// Implementing IReadOnlyList<T>, IList<T>, ICollection<T> and IImmutableList<T>.

// IReadOnlyList<T> is implemented implicitely, with the exception of stuff from 
// IEnumerable, where we leverage the technique used by ImmutableArray<T> to avoid
// allocations and try-finallys.

// IList and ICollection make no sense for this type, however, some optimizations in LINQ use these
// to get the Count property. ToArray, ToList and Reverse also benefit from ICollection.CopyTo.
// We don't want people stumbling unto methods that throw NotSupportedExceptions though,
// so the best we can do is implement these interfaces explicitely.
// This is also really important so we get compilation errors on new Block<int> { 1, 2, 3 }.

// IImmutableList<T> is not a great interface for us, with overloads taking IEqualityComparer<T>.
// All the members also return IImmutableList<T>s rather than Block<T>, and cannot be implemented
// by members returning Block<T>.
// That said, I feel like it would be a shame not to be compatible with it
// since we get it for free, so it's implemented explicitely.

public partial class Block<T>
{
    #region IReadOnlyList<T>

    /// <inheritdoc />
    public T this[int index] =>
        _arr[index];

    /// <inheritdoc />
    public int Count =>
        _arr.Length;

    /// <summary>
    /// Returns an enumerator for the contents of the array.
    /// </summary>
    /// <returns>An enumerator.</returns>
    public ImmutableArray<T>.Enumerator GetEnumerator() =>
        // Leveraging ImmutableArray's highly optimized Enumerator implementation.
        _arr.GetEnumerator();

    /// <summary>
    /// Returns an enumerator for the contents of the array.
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() =>
        _arr.AsEnumerable().GetEnumerator();

    /// <summary>
    /// Returns an enumerator for the contents of the array.
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        _arr.AsEnumerable().GetEnumerator();

    #endregion

    #region ICollection<T>

    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    void ICollection<T>.Clear() => throw new NotSupportedException();

    bool ICollection<T>.Contains(T item) =>
        _arr.Contains(item);

    /// <inheritdoc cref="ImmutableArray{T}.CopyTo(T[], int)"/>
    /// <remarks>See also <see cref="Block{T}.Slice(int, int)"/> or ToArray().</remarks>
    public void CopyTo(T[] destination, int destinationIndex) =>
        // This implements ICollection<T>.CopyTo. It could be useful as an alternative to slicing
        // for low-allocation code, and I don't want to give anyone a good reason to cast to ICollection<T>,
        // so we expose it.
        // We add the other overloads from ImmutableArray because why not?
        // Overloads don't pollute IntelliSense too much.
        _arr.CopyTo(destination, destinationIndex);

    /// <inheritdoc cref="ImmutableArray{T}.CopyTo(T[])"/>
    /// <remarks>See also <see cref="Block{T}.Slice(int, int)"/> or ToArray().</remarks>
    public void CopyTo(T[] destination) =>
        _arr.CopyTo(destination);

    /// <inheritdoc cref="ImmutableArray{T}.CopyTo(int, T[], int, int)"/>
    /// <remarks>See also <see cref="Block{T}.Slice(int, int)"/> or ToArray().</remarks>
    public void CopyTo(int sourceIndex, T[] destination, int destinationIndex, int length) =>
        _arr.CopyTo(sourceIndex, destination, destinationIndex, length);

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

    bool ICollection<T>.IsReadOnly => true;

    #endregion

    #region IList<T>

    T IList<T>.this[int index]
    {
        get => _arr[index];
        set => throw new NotSupportedException();
    }

    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

    #endregion

    #region IImmutableList<T>

    IImmutableList<T> IImmutableList<T>.Clear() =>
        Empty;

    IImmutableList<T> IImmutableList<T>.Add(T value) =>
        Append(value);

    IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items) =>
        Append(items);

    IImmutableList<T> IImmutableList<T>.Insert(int index, T element) =>
        Insert(index, element);

    IImmutableList<T> IImmutableList<T>.InsertRange(int index, IEnumerable<T> items) =>
        Insert(index, items);

    IImmutableList<T> IImmutableList<T>.Remove(T value, IEqualityComparer<T> equalityComparer) =>
        _arr.Remove(value, equalityComparer).ToBlock();

    IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T> match) =>
        _arr.RemoveAll(match).ToBlock();

    IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer) =>
        _arr.RemoveRange(items, equalityComparer).ToBlock();

    IImmutableList<T> IImmutableList<T>.RemoveRange(int index, int count) =>
        _arr.RemoveRange(index, count).ToBlock();

    IImmutableList<T> IImmutableList<T>.RemoveAt(int index) =>
        RemoveAt(index);

    IImmutableList<T> IImmutableList<T>.SetItem(int index, T value) =>
        SetItem(index, value);

    IImmutableList<T> IImmutableList<T>.Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer) =>
        _arr.Replace(oldValue, newValue, equalityComparer).ToBlock();

    int IImmutableList<T>.IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer) =>
        _arr.IndexOf(item, index, count, equalityComparer);

    int IImmutableList<T>.LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer) =>
        _arr.LastIndexOf(item, index, count, equalityComparer);

    #endregion
}
