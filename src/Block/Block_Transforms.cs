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

    // Don't use -Range suffix but rely on overloading instead. This simplifies the interface.
    // There's no issue with generic resolution as the generic type is not inferred from the argument (as it is in Block.Create).

    // I'm not a huge fan of the Remove(T item), Remove(T[] items) methods on ImmutableArray, they only remove the first occurence
    // and that's counter-intuitive to me. Should be called RemoveFirst. And then are they that useful?
    // We have Where for general-purpose filtering.
    // RemoveAll has an optimization where it builds a set of indices to remove, then it's able to allocate a single result array.
    // Seems only useful for arrays of structs. Should benchmark it on general cases vs .Where().ToBlock().

    // 

    /// <inheritdoc cref="ImmutableArray{T}.Add(T)"/>
    public Block<T> Append(T item) =>
        new(_arr.Add(item));

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(IEnumerable{T})"/>
    public Block<T> Append(IEnumerable<T> items) =>
        new(_arr.AddRange(items));

    /// <inheritdoc cref="ImmutableArray{T}.AddRange(ImmutableArray{T})"/>
    public Block<T> Append(Block<T> items) =>
        new(_arr.AddRange(items._arr));

    /// <inheritdoc cref="ImmutableArray{T}.Insert(int, T)"/>
    public Block<T> Insert(int index, T item) =>
        new(_arr.Insert(index, item));

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, IEnumerable{T})"/>
    public Block<T> Insert(int index, IEnumerable<T> items) =>
        new(_arr.InsertRange(index, items));

    /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, ImmutableArray{T})"/>
    public Block<T> Insert(int index, Block<T> items) =>
        new(_arr.InsertRange(index, items._arr));

    /// <inheritdoc cref="ImmutableArray{T}.RemoveAt(int)"/>
    public Block<T> RemoveAt(int index) =>
        new(_arr.RemoveAt(index));

    ///// <inheritdoc cref="ImmutableArray{T}.RemoveRange(IEnumerable{T})"/>
    //public Block<T> RemoveFirst(IEnumerable<T> items) =>
    //    new(_arr.RemoveRange(items));

    ///// <inheritdoc cref="ImmutableArray{T}.Remove(T, IEqualityComparer{T}?)"/>
    //public Block<T> RemoveFirst(T item) =>
    //    new(_arr.Remove(item));

    /// <inheritdoc cref="ImmutableArray{T}.SetItem(int, T)"/>
    public Block<T> SetItem(int index, T item) =>
        new(_arr.SetItem(index, item));
}
