using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ValueCollections;

public partial class Block<T> : IReadOnlyList<T>
{
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
    //public ImmutableArray<T>.Enumerator GetEnumerator() =>
        // Leveraging ImmutableArray's highly optimized Enumerator implementation.
    //    throw new NotImplementedException();
        //_arr.GetEnumerator();

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
}