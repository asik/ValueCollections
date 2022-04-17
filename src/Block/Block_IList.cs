using System;
using System.Collections.Generic;
using System.Text;

namespace ValueCollections;

// IList/ICollection: it makes no sense for this type, however, some optimizations in LINQ use these
// to get the Count property. We don't want people using these methods though, so the best we can
// do is implement the interface explicitely.

public partial class Block<T>
{
    bool ICollection<T>.IsReadOnly => true;

    T IList<T>.this[int index]
    {
        get => _arr[index];
        set => throw new NotSupportedException();
    }

    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    void ICollection<T>.Clear() => throw new NotSupportedException();

    bool ICollection<T>.Contains(T item) =>
        _arr.Contains(item);

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) =>
        _arr.CopyTo(array, arrayIndex);

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}
