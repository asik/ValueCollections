using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ValueCollections
{
    public readonly struct Block<T> : 
        IReadOnlyList<T>, 
        IEquatable<Block<T>>
    {
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
        // See if equality perf via IStructuralEquatable is ok
        // Questionable interfaces:
        // - ICollection - for legacy stuff, it's not really an issue to implement though
        // - IList - for legacy stuff, but would violate LSP. ImmutableArray does it. Should we?
        // - IStructuralEquality - do we need this if we're already structurally comparable via IEquatable?
        // - IStructuralComparable - does it really make sense to order arrays? What's the use case? OrderedSet? F# does it though.

        #region ImmutableArray interface

        public static readonly Block<T> Empty = new Block<T>(ImmutableArray<T>.Empty);

        public int Length =>
            _arr.Length;

        public bool IsDefault =>
            _arr.IsDefault;

        public bool IsDefaultOrEmpty =>
            _arr.IsDefaultOrEmpty;

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
        public ImmutableArray<T>.Enumerator GetEnumerator() =>
            _arr.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            _arr.AsEnumerable().GetEnumerator();

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