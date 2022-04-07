using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;
using ValueCollections.Json;

namespace ValueCollections
{
    /// <summary>
    /// An immutable array with value equality. <see href="https://github.com/asik/ValueCollections#readme"/>
    /// </summary>
    [JsonConverter(typeof(BlockJsonConverterFactory))]
    public partial class Block<T> :
        IReadOnlyList<T>,
        IEquatable<Block<T>>,
        IImmutableList<T>
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

        // So it should be based on ImmutableArray.

        // Should it be a reference type or a value type?
        // ImmutableArray is a value type. It does this because it is designed as a low-overhead alternative to
        // ImmutableList. It's also unsafe, throwing InvalidOperationException/NullReferenceException
        // when uninitialized. We could make every function go through an if statement to make it safe, but that would
        // slow down usage.
        // ImmutableArray exposes IsDefault, IsDefaultOrEmpty and IsEmpty. It needs all 3
        // C# now has nullability checking for reference types, making them much safer than they used to be when
        // ImmutableArray was designed. Since they allow us to control our initialization, we can be fast on usage.
        // With nullability checking, reference type also offer a correctness advantage that structs lack: the compiler 
        // warns if they're left uninitialized.
        // However, making it a reference means two allocations per instance instead of one. This means using Block<T>
        // creates more GC pressure, leading to more frequent GC. However, this is true of every collection type outside of
        // plain arrays.

        // Overall, making it a reference type is the better solution.

        readonly ImmutableArray<T> _arr;

        public Block(IEnumerable<T> items) =>
            // ImmutableArray is smart enough to check if it's a finite collection and pre-allocate if possible,
            // so we don't need further overloads for collections.
            _arr = ImmutableArray.CreateRange(items);

        public Block(params T[] items) =>
            // This is to support nice syntax like new Block<int>(1, 2, 3, 4, 5, 6)
            _arr = ImmutableArray.Create(items);

        Block(ImmutableArray<T> items) =>
            // For fast creation; used internally
            _arr = items;

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

        /// <inheritdoc cref="ImmutableArray{T}.Empty"/>
        public static readonly Block<T> Empty = new Block<T>(ImmutableArray<T>.Empty);
        
        /// <inheritdoc cref="ImmutableArray{T}.Length"/>
        public int Length =>
            _arr.Length;

        // We do not support .IsDefault or IsDefaultOrEmpty because this is a reference type and does not support
        // default initialization. Furthermore, IsEmpty seems pretty useless when other common array types do not have it.

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
        IEnumerator IEnumerable.GetEnumerator() =>
            _arr.AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator for the contents of the array.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
            _arr.AsEnumerable().GetEnumerator();

        #endregion

        /// <summary>
        /// Provides support for range indexing in C# 8.0 and later.
        /// Can also be called directly.
        /// </summary>        
        /// <param name="start">The index of the first element in the source array to include in the resulting array.</param>
        /// <param name="length">The number of elements from the source array to include in the resulting array.</param>
        public Block<T> Slice(int start, int length) =>
            new Block<T>(ImmutableArray.Create(_arr, start, length));

        // The naive approach to comparison doesn't work, running into
        // System.ArgumentException : Object is not a array with the same number of elements as the array to compare it to.
        //public int CompareTo(Block<T> other) =>
        //    ((IStructuralComparable)_arr.ToArray()).CompareTo(other._arr.ToArray(), Comparer<T>.Default);

        //public static bool operator <(Block<T> left, Block<T> right) =>
        //    left.CompareTo(right) < 0;

        //public static bool operator >(Block<T> left, Block<T> right) =>
        //    left.CompareTo(right) > 0;

    }

    /// <summary>
    /// A set of initialization methods for instances of <see cref="Block{T}"/>.
    /// </summary>
    public static class Block
    {
        /// <summary>
        /// Creates a new <see cref="Block{T}"/> from a sequence of items.
        /// </summary>
        /// <typeparam name="T">The type of element stored in the array.</typeparam>
        /// <param name="items">The elements to store in the array.</param>
        /// <returns>A <see cref="Block{T}"/> containing the items provided.</returns>
        public static Block<T> CreateRange<T>(IEnumerable<T> items) =>
            new Block<T>(items);

        /// <summary>
        /// Creates a new <see cref="Block{T}"/> from an array of items.
        /// </summary>
        /// <typeparam name="T">The type of element stored in the array.</typeparam>
        /// <param name="items">The elements to store in the array.</param>
        /// <returns>A <see cref="Block{T}"/> containing the items provided.</returns>
        public static Block<T> Create<T>(params T[] items) =>
            new Block<T>(items);

        /// <summary>
        /// Creates a new <see cref="Block{T}"/> from a sequence of items.
        /// </summary>
        /// <typeparam name="T">The type of element stored in the array.</typeparam>
        /// <param name="items">The elements to store in the array.</param>
        /// <returns>A <see cref="Block{T}"/> containing the items provided.</returns>
        public static Block<T> ToBlock<T>(this IEnumerable<T> items) =>
            new Block<T>(items);
    }
}
