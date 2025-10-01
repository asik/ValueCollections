using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ValueCollections;

public partial class ValueArray<T> : IReadOnlyList<T>
{
    /// <inheritdoc />
    public T this[int index] =>
        _arr[index];

    /// <inheritdoc />
    public int Count =>
        _arr.Length;

    /// <summary>
    /// Returns an enumerator for the contents of the array.
    /// </summary>
    /// <remarks>
    /// This is an optimization. The type works fine without this. 
    /// It eliminates the overhead of an enumerator object in this common scenario:
    /// <code>
    /// foreach(ValueArray{T} item in valueArray) {}
    /// </code>
    /// </remarks>
    /// <returns>An enumerator.</returns>
    public Enumerator GetEnumerator() =>
        new(_arr);

    /// <summary>
    /// Returns an enumerator for the contents of the array.
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator IEnumerable.GetEnumerator() =>
        _arr.GetEnumerator();

    /// <summary>
    /// Returns an enumerator for the contents of the array.
    /// </summary>
    /// <returns>An enumerator.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() =>
        _arr.AsEnumerable().GetEnumerator();

    /// <summary>
    /// An array enumerator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is important that this enumerator does NOT implement <see cref="IDisposable"/>.
    /// We want the iterator to inline when we do foreach and to not result in
    /// a try/finally frame in the client.
    /// </para>
    /// <para>
    /// This type and its comments are pulled wholesale from ImmutableArray.
    /// </para>
    /// </remarks>
    public struct Enumerator
    {
        /// <summary>
        /// The array being enumerated.
        /// </summary>
        readonly T[] _array;

        /// <summary>
        /// The currently enumerated position.
        /// </summary>
        /// <value>
        /// -1 before the first call to <see cref="MoveNext"/>.
        /// >= this.array.Length after <see cref="MoveNext"/> returns false.
        /// </value>
        int _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// </summary>
        /// <param name="array">The array to enumerate.</param>
        internal Enumerator(T[] array)
        {
            _array = array;
            _index = -1;
        }

        /// <summary>
        /// Gets the currently enumerated value.
        /// </summary>
        public T Current =>
            // PERF: no need to do a range check, we already did in MoveNext.
            // if user did not call MoveNext or ignored its result (incorrect use)
            // they will still get an exception from the array access range check.
            _array[_index];

        /// <summary>
        /// Advances to the next value to be enumerated.
        /// </summary>
        /// <returns><c>true</c> if another item exists in the array; <c>false</c> otherwise.</returns>
        public bool MoveNext() =>
            ++_index < _array.Length;
    }
}