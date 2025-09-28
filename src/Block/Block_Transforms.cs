using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

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
        new(_arr.Add(item));

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(IEnumerable{T})"/>
    public Block<T> Append(IEnumerable<T> items) =>
        _arr.AddRange(items).ToBlock();

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(ImmutableArray{T})"/>
    public Block<T> Append(Block<T> items) =>
        _arr.AddRange(items._arr).ToBlock();

    /// <inheritdoc cref="ImmutableArray{T}.Insert(int, T)"/>
    public Block<T> Insert(int index, T item) =>
        new(_arr.Insert(index, item));

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, IEnumerable{T})"/>
    public Block<T> Insert(int index, IEnumerable<T> items) =>
        _arr.InsertRange(index, items).ToBlock();

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, ImmutableArray{T})"/>
    public Block<T> Insert(int index, Block<T> items) =>
        _arr.InsertRange(index, items._arr).ToBlock();

    /// <inheritdoc cref="ImmutableArray{T}.RemoveAt(int)"/>
    public Block<T> RemoveAt(int index) =>
        _arr.RemoveAt(index).ToBlock();

    /// <inheritdoc cref="ImmutableArray{T}.SetItem(int, T)"/>
    public Block<T> SetItem(int index, T item) =>
        new(_arr.SetItem(index, item));
}
