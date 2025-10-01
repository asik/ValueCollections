using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ValueCollections.Unsafe;

namespace ValueCollections;

public partial class ValueArray<T>
{
    // General ideas:
    // Try to rely on LINQ where it makes sense, but benchmark for important things that could benefit from optimization.
    // e.g. sorting, filtering

    // Do NOT have a method called Add(T), because that enables `new ValueArray<T> { 1, 2, 3 }` which would not work.

    // I'm not a huge fan of the Remove(T item), Remove(T[] items) methods on ImmutableArray, they only remove the first occurence
    // and that's counter-intuitive to me. Should be called RemoveFirst. And then are they that useful?
    // We have Where for general-purpose filtering.
    // RemoveAll has an optimization where it builds a set of indices to remove, then it's able to allocate a single result array.
    // Seems only useful for arrays of structs. Should benchmark it on general cases vs .Where().ToValueArray().


    /// <summary>
    /// Provides support for range indexing in C# 8.0 and later.
    /// Can also be called directly.
    /// </summary>
    /// <param name="start">The index of the first element in the source array to include in the resulting array.</param>
    /// <param name="length">The number of elements from the source array to include in the resulting array.</param>
    public ValueArray<T> Slice(int start, int length) =>
        new(ImmutableArray.Create(_arr, start, length));

    /// <inheritdoc cref="ImmutableArray{T}.Add(T)"/>
    public ValueArray<T> Add(T item) =>
        ValueCollectionsMarshal.AsValueArray([.. _arr, item]);

    // TODO consider adding a ReadOnlySpan overload

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(IEnumerable{T})"/>
    public ValueArray<T> AddRange(IEnumerable<T> items) =>
        ValueCollectionsMarshal.AsValueArray([.. _arr, ..items]);

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(ImmutableArray{T})"/>
    public ValueArray<T> AddRange(ValueArray<T> items) =>
        items.Length == 0 
            ? this 
            : ValueCollectionsMarshal.AsValueArray([.. _arr, ..items._arr]);

    /// <inheritdoc cref="ImmutableArray{T}.Insert(int, T)"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public ValueArray<T> Insert(int index, T item)
    {
        ThrowIfIndexInvalidForInsert(index);
        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index, _arr.Length - index);
        return ValueCollectionsMarshal.AsValueArray([.. elementsBefore, item, .. elementsAfter]);
    }

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, IEnumerable{T})"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public ValueArray<T> InsertRange(int index, IEnumerable<T> items)
    {
        ThrowIfIndexInvalidForInsert(index);
        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index, _arr.Length - index);
        return ValueCollectionsMarshal.AsValueArray([..elementsBefore, ..items, ..elementsAfter]);
    }

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, ImmutableArray{T})"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public ValueArray<T> InsertRange(int index, ValueArray<T> items)
    {
        if (items.Length == 0)
        {
            return this;
        }

        ReadOnlySpan<int> span = new[] { 1, 2, 3 };// ImmutableArray.Create(1, 2, 3);

        ThrowIfIndexInvalidForInsert(index);
        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index, _arr.Length - index);
        return ValueCollectionsMarshal.AsValueArray([.. elementsBefore, ..items, ..elementsAfter]);
    }

    /// <inheritdoc cref="ImmutableArray{T}.RemoveAt(int)"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public ValueArray<T> RemoveAt(int index)
    {
        ThrowOutRangeIfNotInBounds(index);
        if (_arr.Length == 1)
        {
            return Empty;
        }

        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index + 1, _arr.Length - index - 1);
        return ValueCollectionsMarshal.AsValueArray([.. elementsBefore, .. elementsAfter]);
    }

    /// <inheritdoc cref="ImmutableArray{T}.SetItem(int, T)"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public ValueArray<T> SetItem(int index, T item)
    {
        ThrowOutRangeIfNotInBounds(index);
        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index + 1, _arr.Length - index - 1);
        return ValueCollectionsMarshal.AsValueArray([.. elementsBefore, item, .. elementsAfter]);
    }

    /// <summary>
    /// Sorts the array, according to the specified comparer, or to the default comparer if none is provided.
    /// </summary>
    /// <param name="comparer">An optional comparer to use when comparing elements. If null, the default comparer for type T is used.</param>
    /// <returns>A new <see cref="ValueArray{T}"/> with the elements sorted.</returns>
    public ValueArray<T> Sort(IComparer<T>? comparer = null)
    {
        if (Count == 0)
        {
            return Empty;
        }

        var arrCopy = _arr.ToArray();
        Array.Sort(arrCopy, comparer ?? Comparer<T>.Default);
        return ValueCollectionsMarshal.AsValueArray(arrCopy);
    }

    void ThrowIfIndexInvalidForInsert(int index)
    {
        if (index < 0 || index > _arr.Length)
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    void ThrowOutRangeIfNotInBounds(int index)
    {
        if (index < 0 || index >= _arr.Length)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
