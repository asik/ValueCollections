using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ValueCollections;

public partial class Block<T>
{
    static void RequiresNonNull(object param, string paramName)
    {
        if (param == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <inheritdoc cref="ImmutableArray{T}.IndexOf(T)"/>
    public int IndexOf(T item) =>
        _arr.IndexOf(item);

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


}
