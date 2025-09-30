using System;
using System.Collections.Immutable;
namespace ValueCollections;

// Adding some methods from the Array class.
// The rationale for inclusion here would be any combination of:
// - The method is array-specific e.g. index operations
// - It's not available in System.Linq
// - If available in System.Linq, it can be made much faster by specializing it for our type

public partial class Block<T>
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
        Array.LastIndexOf(_arr, item);


    /// <param name="match">
    /// A function that takes in an item 
    /// and returns whether it is the one we are looking for.
    /// </param>
    /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
    public int FindIndex(Func<T, bool> match)
    {
        RequiresNonNull(match, nameof(match));

        for (var i = 0; i<_arr.Length; i++)
        {
            if (match(_arr[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary> 
    /// Searches for an element that matches the conditions defined by the specified
    /// predicate, and returns the zero-based index of the first occurrence within the
    /// entire <see cref="Block{T}"/>.
    /// </summary>
    /// <param name="match">
    /// A function that takes in an item and its index, 
    /// and returns whether it is the one we are looking for.
    /// </param>
    /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
    public int FindIndex(Func<T, int, bool> match)
    {
        RequiresNonNull(match, nameof(match));

        for (var i = 0; i < _arr.Length; i++)
        {
            if (match(_arr[i], i))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary> 
    /// Searches for an element that matches the conditions defined by the specified
    /// predicate, and returns the zero-based index of the last occurrence within the
    /// entire <see cref="Block{T}"/>.
    /// </summary>
    /// <param name="match">
    /// A function that takes in an item, and returns whether it is the one we are looking for.
    /// </param>
    /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
    public int FindLastIndex(Func<T, bool> match)
    {
        RequiresNonNull(match, nameof(match));

        for (var i = _arr.Length - 1; i >= 0; i--)
        {
            if (match(_arr[i]))
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary> 
    /// Searches for an element that matches the conditions defined by the specified
    /// predicate, and returns the zero-based index of the last occurrence within the
    /// entire <see cref="Block{T}"/>.
    /// </summary>
    /// <param name="match">
    /// A function that takes in an item and its index, and returns whether it is the one we are looking for.
    /// </param>
    /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
    public int FindLastIndex(Func<T, int, bool> match)
    {
        RequiresNonNull(match, nameof(match));

        for (var i = _arr.Length - 1; i >= 0; i--)
        {
            if (match(_arr[i], i))
            {
                return i;
            }
        }
        return -1;
    }
}
