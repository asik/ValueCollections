using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using ValueCollections.Json;

namespace ValueCollections
{
    /// <inheritdoc/>
    /// <summary>
    /// An immutable array with value equality. <see href="https://github.com/asik/ValueCollections#readme"/>
    /// </summary>
    [JsonConverter(typeof(BlockJsonConverterFactory))]
    public readonly struct Block<T> :
        IReadOnlyList<T>,
        IEquatable<Block<T>>
    {
        // Should we be based on ImmutableArray or ImmutableList? After all, we're going to support IImmutableList,
        // and ImmutableList is a better type for that purpose, optimized for adding/removing.
        // Why select a type that's going to be slow on updates if we want to support updates?
        // Rationale: if update performance is important, are you going to be happy with ImmutableList?
        // Or are you going to choose List<T> or T[] or something imperative.
        // I think the vast majority of use cases for this type will be simple updates where a single array allocation is fine.
        // For lots of updates where perf matters, you can use something mutable.
        // Advantages of ImmutableArray that we would lose by going to ImmutableList:
        // - Consistency with planned F# Block type
        // - Array-like performance in creation, iteration
        // - 0 GC overhead over the underlying array (less overhead than List<T>!)
        // 
        readonly ImmutableArray<T> _arr;

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
        // Performance work:
        // - is equality via IStructuralEquatable ok? Can we do better when T: IEquatable<T>?
        // - can we improve serialization/deserialization perf with a constructor for BlockJsonConverter that takes in JsonSerializerOptions?
        //   https://makolyte.com/dotnet-jsonserializer-is-over-200x-faster-if-you-reuse-jsonserializeroptions/
        // Questionable interfaces:
        // - ICollection - this is a terrible interface. We don't need it to support System.Text.Json. Any real need to support it?
        // - IList - for mutable indexable collections e.g. System.Collections.Generic.List. Makes 0 sense for us.
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