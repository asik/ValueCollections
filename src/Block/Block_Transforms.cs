using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ValueCollections.Unsafe;

namespace ValueCollections;

public partial class Block<T>
{
    // General ideas:
    // Try to rely on LINQ where it makes sense, but benchmark for important things that could benefit from optimization.
    // e.g. sorting, filtering

    // Do NOT have a method called Add(T), because that enables `new Block<T> { 1, 2, 3 }` which would not work.

    // I'm not a huge fan of the Remove(T item), Remove(T[] items) methods on ImmutableArray, they only remove the first occurence
    // and that's counter-intuitive to me. Should be called RemoveFirst. And then are they that useful?
    // We have Where for general-purpose filtering.
    // RemoveAll has an optimization where it builds a set of indices to remove, then it's able to allocate a single result array.
    // Seems only useful for arrays of structs. Should benchmark it on general cases vs .Where().ToBlock().


    /// <summary>
    /// Provides support for range indexing in C# 8.0 and later.
    /// Can also be called directly.
    /// </summary>
    /// <param name="start">The index of the first element in the source array to include in the resulting array.</param>
    /// <param name="length">The number of elements from the source array to include in the resulting array.</param>
    public Block<T> Slice(int start, int length) =>
        new(ImmutableArray.Create(_arr, start, length));

    /// <inheritdoc cref="ImmutableArray{T}.Add(T)"/>
    public Block<T> Append(T item) =>
        ValueCollectionsMarshal.AsBlock([.. _arr, item]);

    // TODO consider adding a ReadOnlySpan overload

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(IEnumerable{T})"/>
    public Block<T> Append(IEnumerable<T> items) =>
        ValueCollectionsMarshal.AsBlock([.. _arr, ..items]);

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(ImmutableArray{T})"/>
    public Block<T> Append(Block<T> items) =>
        items.Length == 0 
            ? this 
            : ValueCollectionsMarshal.AsBlock([.. _arr, ..items._arr]);

    /// <inheritdoc cref="ImmutableArray{T}.Insert(int, T)"/>
    public Block<T> Insert(int index, T item)
    {
        ThrowIfIndexInvalidForInsert(index);
        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index, _arr.Length - index);
        return ValueCollectionsMarshal.AsBlock([.. elementsBefore, item, .. elementsAfter]);
    }

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, IEnumerable{T})"/>
    public Block<T> Insert(int index, IEnumerable<T> items)
    {
        ThrowIfIndexInvalidForInsert(index);
        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index, _arr.Length - index);
        return ValueCollectionsMarshal.AsBlock([..elementsBefore, ..items, ..elementsAfter]);
    }

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, ImmutableArray{T})"/>
    public Block<T> Insert(int index, Block<T> items)
    {
        if (items.Length == 0)
        {
            return this;
        }

        ThrowIfIndexInvalidForInsert(index);
        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index, _arr.Length - index);
        return ValueCollectionsMarshal.AsBlock([.. elementsBefore, ..items, ..elementsAfter]);
    }
    //_arr.InsertRange(index, items._arr).ToBlock();

    /// <inheritdoc cref="ImmutableArray{T}.RemoveAt(int)"/>
    public Block<T> RemoveAt(int index)
    {
        ThrowIndexOfOutRangeIfNotInBounds(index);
        if (_arr.Length == 1)
        {
            return Empty;
        }

        var elementsBefore = _arr.AsSpan(0, index);
        var elementsAfter = _arr.AsSpan(index + 1, _arr.Length - index - 1);
        return ValueCollectionsMarshal.AsBlock([.. elementsBefore, .. elementsAfter]);
    }

    /// <inheritdoc cref="ImmutableArray{T}.SetItem(int, T)"/>
    public Block<T> SetItem(int index, T item) =>
        throw new NotImplementedException();
    //new(_arr.SetItem(index, item));


    void ThrowIfIndexInvalidForInsert(int index)
    {
        if (index < 0 || index > _arr.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }

    void ThrowIndexOfOutRangeIfNotInBounds(int index)
    {
        if (index < 0 || index >= _arr.Length)
        {
            throw new IndexOutOfRangeException();
        }
    }
}
