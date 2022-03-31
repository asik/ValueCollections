using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace ValueCollections
{
    /// <inheritdoc/>
    /// <summary>
    /// An immutable array with value equality. <see href="https://github.com/asik/ValueCollections#readme"/>
    /// </summary>
    /// <remarks>
    /// Usage remarks: do not use as an ICollection(T); this interface is only implemented to support deserialization.
    /// </remarks>
    public struct Block<T> :
        IReadOnlyList<T>,
        IEquatable<Block<T>>,
        ICollection<T> // This is only for Deserialization support
    {
        // Not readonly for deserialization, see ICollection implementation.
        ImmutableArray<T> _arr;

        public Block(IEnumerable<T> elems) =>
            // ImmutableArray is smart enough to check if it's a finite collection and pre-allocate if possible,
            // so we don't need further overloads for collections.
            _arr = ImmutableArray.CreateRange(elems);

        public Block(params T[] elems) =>
            // This is to support nice syntax like new Block<int>(1, 2, 3, 4, 5, 6)
            _arr = ImmutableArray.Create(elems);

        // Further optimizations: add single, two, three-element constructors for perf.
        // Add support for IImmutableList
        // Add overloads for LINQ
        // See if equality perf via IStructuralEquatable is ok
        // Questionable interfaces:
        // - ICollection - for legacy stuff, it's not really an issue to implement though
        // - IList - for legacy stuff, but would violate LSP. ImmutableArray does it. Should we?
        // - IStructuralEquality - do we need this if we're already structurally comparable via IEquatable?
        // - IStructuralComparable - does it really make sense to order arrays? What's the use case? OrderedSet? F# does it though.

        #region ImmutableArray interface

        /// <summary>
        /// Gets an empty immutable array.
        /// </summary>
        public static readonly Block<T> Empty = new Block<T>(ImmutableArray<T>.Empty);

        /// <summary>
        /// Gets the number of elements in the array.
        /// </summary>
        public int Length =>
            _arr.Length;

        /// <summary>
        /// Gets a value indicating whether this struct was initialized without an actual array instance.
        /// </summary>
        public bool IsDefault =>
            _arr.IsDefault;

        /// <summary>
        /// Gets a value indicating whether this struct is empty or uninitialized.
        /// </summary>
        public bool IsDefaultOrEmpty =>
            _arr.IsDefaultOrEmpty;

        /// <summary>
        /// Gets a value indicating whether this collection is empty.
        /// </summary>
        public bool IsEmpty =>
            _arr.IsEmpty;

        #endregion

        #region IEquatable<T>

        public bool Equals(Block<T> other) =>
            ((IStructuralEquatable)_arr).Equals(other._arr, EqualityComparer<T>.Default);

        public override bool Equals(object obj) =>
            obj is Block<T> other && Equals(other);

        public override int GetHashCode() =>
            ((IStructuralEquatable)_arr).GetHashCode(EqualityComparer<T>.Default);

        public static bool operator ==(Block<T> left, Block<T> right) =>
            left.Equals(right);

        public static bool operator !=(Block<T> left, Block<T> right) =>
            !left.Equals(right);

        #endregion

        #region IReadOnlyList<T>

        public T this[int index] =>
            _arr[index];

        public int Count =>
            _arr.Length;

        /// <summary>
        /// Returns an enumerator for the contents of the array.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public ImmutableArray<T>.Enumerator GetEnumerator() =>
            _arr.GetEnumerator();

        /// <summary>
        /// Returns an enumerator for the contents of the array.
        /// </summary>
        /// <returns>An enumerator.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        IEnumerator IEnumerable.GetEnumerator() =>
            _arr.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator for the contents of the array.
        /// </summary>
        /// <returns>An enumerator.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="IsDefault"/> property returns true.</exception>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            _arr.AsEnumerable().GetEnumerator();

        #endregion

        #region ICollection<T>

        /// <summary>
        /// The only extension point for collections in System.Text.Json is to allow adding elements via ICollection.Add,
        /// and we also must lie about being read-only. This effectively makes our type mutable by casting it to ICollection.
        /// We can at least make this less discoverable and trigger an error if it's used directly on the type.
        /// https://github.com/dotnet/runtime/issues/67361
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For internal use only.", true)]        
        public bool IsReadOnly => false; 

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For internal use only.", true)]
        public void Add(T item) => 
            _arr = _arr.IsDefault ? ImmutableArray.Create(item) : _arr.Add(item);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For internal use only.", true)]
        public void Clear() => 
            throw new NotSupportedException();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For internal use only.", true)]
        public bool Contains(T item) =>
            throw new NotSupportedException();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For internal use only.", true)]
        public void CopyTo(T[] array, int arrayIndex) =>
            throw new NotSupportedException();

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("For internal use only.", true)]
        public bool Remove(T item) => 
            throw new NotSupportedException();

        #endregion

        // The naive approach to comparison doesn't work, running into
        // System.ArgumentException : Object is not a array with the same number of elements as the array to compare it to.
        //public int CompareTo(Block<T> other) =>
        //    ((IStructuralComparable)_arr.ToArray()).CompareTo(other._arr.ToArray(), Comparer<T>.Default);

        //public static bool operator <(Block<T> left, Block<T> right) =>
        //    left.CompareTo(right) < 0;

        //public static bool operator >(Block<T> left, Block<T> right) =>
        //    left.CompareTo(right) > 0;
    }

    public static class Block
    {
        public static Block<T> CreateRange<T>(IEnumerable<T> elems) =>
            new Block<T>(elems);

        public static Block<T> Create<T>(params T[] elems) =>
            new Block<T>(elems);

        public static Block<T> ToBlock<T>(this IEnumerable<T> elems) =>
            CreateRange(elems);
    }
}