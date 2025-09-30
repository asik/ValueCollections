using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ValueCollections;

// We support this one mainly because LINQ tests if something is an ICollection to get a Count property
// and avoid enumerating in some cases. It also uses ICollection.CopyTo. 
// Of course we can't support any of the mutating methods on this.

public partial class Block<T> : ICollection<T>
{
    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    void ICollection<T>.Clear() => throw new NotSupportedException();

    bool ICollection<T>.Contains(T item) =>
        this.Contains(item);
    // TODO consider adding Block<T>.Contains.
    // System.Linq.Enumerable.Contains will correctly call this, but at the cost of a type check.

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
        Array.Copy(
            sourceArray: _arr, 
            sourceIndex: sourceIndex, 
            destinationArray: destination, 
            destinationIndex: destinationIndex, 
            length: length);

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

    bool ICollection<T>.IsReadOnly => true;
}