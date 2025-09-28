using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ValueCollections;



// Similar to ICollection, we support this to benefit from hardcoded optimizations in LINQ,
// but of course we can't support the mutating members.

public partial class Block<T> : IList<T>
{
    /// <inheritdoc cref="ImmutableArray{T}.IndexOf(T)"/>
    public int IndexOf(T item) =>
        _arr.IndexOf(item);

    T IList<T>.this[int index]
    {
        get => _arr[index];
        set => throw new NotSupportedException();
    }

    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();
}