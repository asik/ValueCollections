using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ValueCollections
{
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

        /// <inheritdoc cref="ImmutableArray{T}.Add(T)"/>
        public Block<T> Append(T item) =>
            new Block<T>(_arr.Add(item));

        /// <inheritdoc cref="ImmutableArray{T}.AddRange(IEnumerable{T})"/>
        public Block<T> Append(IEnumerable<T> items) => 
            new Block<T>(_arr.AddRange(items));

        /// <inheritdoc cref="ImmutableArray{T}.AddRange(ImmutableArray{T}))"/>
        public Block<T> Append(Block<T> items) =>
            new Block<T>(_arr.AddRange(items._arr));

        /// <inheritdoc cref="ImmutableArray{T}.Insert(int, T)"/>
        public Block<T> Insert(int index, T item) =>
            new Block<T>(_arr.Insert(index, item));

        /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, IEnumerable{T})"/>
        public Block<T> Insert(int index, IEnumerable<T> items) =>
            new Block<T>(_arr.InsertRange(index, items));

        /// <inheritdoc cref="ImmutableArray{T}.InsertRange(int, ImmutableArray{T})"/>
        public Block<T> Insert(int index, Block<T> items) =>
            new Block<T>(_arr.InsertRange(index, items._arr));

        /// <inheritdoc cref="ImmutableArray{T}.RemoveAt(int)"/>
        public Block<T> RemoveAt(int index) =>
            new Block<T>(_arr.RemoveAt(index));

        /// <inheritdoc cref="ImmutableArray{T}.SetItem(int, T)"/>
        public Block<T> SetItem(int index, T item) =>
            new Block<T>(_arr.SetItem(index, item));

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
            new Block<T>(_arr.Remove(value, equalityComparer));

        IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T> match) =>
            new Block<T>(_arr.RemoveAll(match));

        IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer) =>
            new Block<T>(_arr.RemoveRange(items, equalityComparer));

        IImmutableList<T> IImmutableList<T>.RemoveRange(int index, int count) =>
            new Block<T>(_arr.RemoveRange(index, count));

        IImmutableList<T> IImmutableList<T>.RemoveAt(int index) =>
            new Block<T>(_arr.RemoveAt(index));

        IImmutableList<T> IImmutableList<T>.SetItem(int index, T value) =>
            SetItem(index, value);

        IImmutableList<T> IImmutableList<T>.Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer) =>
            new Block<T>(_arr.Replace(oldValue, newValue, equalityComparer));

        int IImmutableList<T>.IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer) =>
            _arr.IndexOf(item, index, count, equalityComparer);

        int IImmutableList<T>.LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer) =>
            _arr.LastIndexOf(item, index, count, equalityComparer);
    }
}
