using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ValueCollections;

// IImmutableList<T> is not a great interface for us, with overloads taking IEqualityComparer<T>.
// All the members also return IImmutableList<T>s rather than Block<T>, and cannot be implemented
// by members returning Block<T>.
// That said, I feel like it would be a shame not to be compatible with it
// since we get it for free, so it's implemented explicitely.

public partial class Block<T> : IImmutableList<T>
{
    static void RequiresNonNull(object param, string paramName)
    {
        if (param == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <inheritdoc cref="ImmutableArray{T}.LastIndexOf(T)"/>
    public int LastIndexOf(T item) =>
        _arr.LastIndexOf(item);

    /// <summary>
    /// Searches for the first item that satisfies the given predicate.
    /// </summary>
    /// <param name="predicate">
    /// A function that takes in an item 
    /// and returns whether it is the one we are looking for.
    /// </param>
    /// <returns>
    /// The 0-based index into the array where the item was found; 
    /// or -1 if it could not be found.
    /// </returns>
    public int FindIndex(Func<T, bool> predicate)
    {
        RequiresNonNull(predicate, nameof(predicate));

        for (var i = 0; i < _arr.Length; i++)
        {
            if (predicate(_arr[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Searches for the first item that satisfies the given predicate. 
    /// The predicate's arguments are the item and its index within the array.
    /// </summary>
    /// <param name="predicate">
    /// A function that takes in an item and its index, 
    /// and returns whether it is the one we are looking for.
    /// </param>
    /// <returns>
    /// The 0-based index into the array where the item was found; 
    /// or -1 if it could not be found.
    /// </returns>
    public int FindIndex(Func<T, int, bool> predicate)
    {
        RequiresNonNull(predicate, nameof(predicate));

        for (var i = 0; i < _arr.Length; i++)
        {
            if (predicate(_arr[i], i))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Searches for the last item that satisfies the given predicate. 
    /// </summary>
    /// <param name="predicate">
    /// A function that takes in an item
    /// and returns whether it is the one we are looking for.
    /// </param>
    /// <returns>
    /// The 0-based index into the array where the item was found; 
    /// or -1 if it could not be found.
    /// </returns>
    public int FindLastIndex(Func<T, bool> predicate)
    {
        RequiresNonNull(predicate, nameof(predicate));

        for (var i = _arr.Length - 1; i >= 0; i--)
        {
            if (predicate(_arr[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Searches for the last item that satisfies the given predicate. 
    /// The predicate's arguments are the item and its index within the array.
    /// </summary>
    /// <param name="predicate">
    /// A function that takes in an item and its index, 
    /// and returns whether it is the one we are looking for.
    /// </param>
    /// <returns>
    /// The 0-based index into the array where the item was found; 
    /// or -1 if it could not be found.
    /// </returns>
    public int FindLastIndex(Func<T, int, bool> predicate)
    {
        RequiresNonNull(predicate, nameof(predicate));

        for (var i = _arr.Length - 1; i >= 0; i--)
        {
            if (predicate(_arr[i], i))
            {
                return i;
            }
        }
        return -1;
    }



    // Explicit implementations
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

}
