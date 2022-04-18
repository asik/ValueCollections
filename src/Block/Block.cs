using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using ValueCollections.Json;

namespace ValueCollections;

/// <summary>
/// An immutable array with value equality. <see href="https://github.com/asik/ValueCollections#readme"/>
/// </summary>
[JsonConverter(typeof(BlockJsonConverterFactory))]
public partial class Block<T> :
    IReadOnlyList<T>,
    IEquatable<Block<T>>,
    IList<T>,
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

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from a sequence of items.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    public Block(IEnumerable<T> items) =>
        // ImmutableArray is smart enough to check if it's a finite collection and pre-allocate if possible,
        // so we don't need further overloads for collections.
        _arr = ImmutableArray.CreateRange(items);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from an array of items.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    public Block(params T[] items) =>
        // This is to support nice syntax like new Block<int>(1, 2, 3, 4, 5, 6)
        _arr = ImmutableArray.Create(items);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from an <see cref="ImmutableArray{T}"/>.
    /// Does not allocate a new array. 
    /// Use this in combination with <see cref="ImmutableArray{T}.Builder.MoveToImmutable"/>
    /// to build an array dynamically without an extra copy at the end to generate the <see cref="Block{T}"/>.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    public Block(ImmutableArray<T> items) =>
        _arr = items;

    // Further optimizations: add single, two, three-element constructors for perf.
    // Add support for IImmutableList
    // Add overloads for LINQ
    // Performance work:
    // - is equality via IStructuralEquatable ok? Can we do better when T: IEquatable<T>? - ANSWER: yes
    // - can we improve serialization/deserialization perf with a constructor for BlockJsonConverter that takes in JsonSerializerOptions?
    //   https://makolyte.com/dotnet-jsonserializer-is-over-200x-faster-if-you-reuse-jsonserializeroptions/
    // Questionable interfaces:
    // - ICollection, IList - these are terrible, but *sigh* some LINQ optimisations rely on these to provide a .Count property.
    // - IStructuralEquality - do we need this if we're already structurally comparable via IEquatable?
    // - IStructuralComparable - does it really make sense to order arrays? What's the use case? OrderedSet? F# does it though.

    /// <inheritdoc cref="ImmutableArray{T}.Empty"/>
    public static readonly Block<T> Empty =
        new(ImmutableArray<T>.Empty);

    /// <inheritdoc cref="ImmutableArray{T}.Length"/>
    public int Length =>
        _arr.Length;

    /// <inheritdoc />
    public override string ToString()
    {
        static string GetNakedTypeName(Type type) =>
            type.IsArray ? "Array" : type.Name.Split('`')[0];

        static string PrintElems(IEnumerable elems)
        {
            var cutoff = 10;
            var elemsArray = elems.Cast<object>().Take(cutoff + 1).ToArray();
            if (elemsArray.Length == 0)
            {
                return "{ }";
            }
            var elementString = string.Join(", ", elemsArray.Take(cutoff).Select(PrintElem));
            if (elemsArray.Length > cutoff)
            {
                elementString += ", ...";
            }
            return $"{{ {elementString} }}";
        }

        static string PrintElem(object elem)
        {
            var ownToString = elem.ToString();
            if (ownToString != elem.GetType().ToString())
            {
                return ownToString;
            }
            if (elem is ICollection coll)
            {
                return $"{GetNakedTypeName(elem.GetType())}({coll.Count}) {PrintElems(coll)}";
            }
            if (elem is IEnumerable enumerable)
            {
                return $"{GetNakedTypeName(elem.GetType())} {PrintElems(enumerable)}";
            }
            return ownToString;
        }

        var elementString = string.Join(", ", _arr.Take(10).Cast<object>().Select(PrintElem));
        if (Length > 10)
        {
            elementString += ", ...";
        }
        elementString = Length == 0 ? "{ }" : $"{{ {elementString} }}";

        return $"Block({Length}) {elementString}";
    }

    // We do not support .IsDefault or IsDefaultOrEmpty because this is a reference type and does not support
    // default initialization. Furthermore, IsEmpty seems pretty useless when other common array types do not have it.

    #region IEquatable<T>

    static readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;

    /// <inheritdoc />
    public bool Equals(Block<T> other)
    {
        // Equality via IStructuralEquatable appears to box every single element.
        // We can do 57X faster and avoid all allocations like this.
        // Note that _arr.SequenceEquals(other._arr) is a very similar implementation to this but seemed about 30% slower.
        if (other is null)
        {
            return false;
        }

        var otherArray = other._arr;
        if (otherArray.Length != _arr.Length)
        {
            return false;
        }

        // Doing two optimizations here, both result in measurably faster code:
        // We cache EqualityComparer<T>.Default in our own static field
        // We copy that static field into a local here because local variable access is faster
        var comparer = _comparer;
        for (var i = 0; i < _arr.Length; ++i)
        {
            if (!comparer.Equals(_arr[i], otherArray[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object obj) =>
        obj is Block<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // This should be an ok implementation.
        // HashCode source can be viewed here https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/HashCode.cs
        // (not sure where the current implementation is, seems hard to find)
        // It's recommended by IDE0070.
        // It does not allocate. It's based on a really fast algorithm.
        // Its hashes are not cryptographic in quality but that's not the purpose of GetHashCode.
        // Its hashes are randomized across two runs of the same program and should not be stored outside of the program.
        // This is to defend against a known attack. See https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        // Also see https://docs.microsoft.com/en-us/archive/blogs/ericlippert/guidelines-and-rules-for-gethashcode

        // Other ideas:
        // - IStructuralEquatable.GetHashCode() only looks at the last 8 elements, so it's basically useless.
        // - Cryptographic hashes like MD5, SHA256: presumably not worth much slower performance.
        // - Copy-pasting some bitwise-operations-based algorithm I don't understand from Stackoverflow or a blog: just no.

        var hashCode = new HashCode();
        for (var i = 0; i < _arr.Length; ++i)
        {
            hashCode.Add(_arr[i]);
        }
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
    public static bool operator ==(Block<T> left, Block<T> right) =>
        left.Equals(right);

    /// <inheritdoc />
    public static bool operator !=(Block<T> left, Block<T> right) =>
        !left.Equals(right);

    #endregion
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
        new(items);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from an array of items.
    /// </summary>
    /// <typeparam name="T">The type of element stored in the array.</typeparam>
    /// <param name="items">The elements to store in the array.</param>
    /// <returns>A <see cref="Block{T}"/> containing the items provided.</returns>
    public static Block<T> Create<T>(params T[] items) =>
        new(items);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from a sequence of items.
    /// </summary>
    /// <typeparam name="T">The type of element stored in the array.</typeparam>
    /// <param name="items">The elements to store in the array.</param>
    /// <returns>A <see cref="Block{T}"/> containing the items provided.</returns>
    public static Block<T> ToBlock<T>(this IEnumerable<T> items) =>
        new(items);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from an <see cref="ImmutableArray{T}"/>.
    /// Does not allocate a new array. 
    /// Use this in combination with <see cref="ImmutableArray{T}.Builder.MoveToImmutable"/>
    /// to build an array dynamically without an extra copy at the end to generate the <see cref="Block{T}"/>.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    /// <returns>A <see cref="Block{T}"/> containing the items provided.</returns>
    public static Block<T> ToBlock<T>(this ImmutableArray<T> items) =>
        new(items);
}
