using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ValueCollections;

// 2025 update
// - F# no longer plans on adding a "Block" type. https://github.com/fsharp/fslang-design/discussions/528#discussioncomment-5595874
//      - Drop the weird name and call this "ValueArray" probably.

/// <summary>
/// An immutable array with value equality. <see href="https://github.com/asik/ValueCollections#readme"/>
/// </summary>
[CollectionBuilder(typeof(Block), nameof(Block.CreateRange))]
public partial class Block<T>
{
    T[] _arr;

    /// <summary>
    /// Exposes the internal array. Exposed publicly through <see cref="Unsafe.ValueCollectionsMarshal"/> only.
    /// </summary>
    internal T[] UnsafeInternalArray => _arr;

    /// <summary>
    /// Unsafe wrapping constructor. Exposed publicly through <see cref="Unsafe.ValueCollectionsMarshal"/> only.
    /// </summary>
    internal Block(T[] arr) =>
        _arr = arr;

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from a sequence of items.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    public Block(IEnumerable<T> items) =>
        _arr = [.. items];

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from a sequence of items.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    public Block(IReadOnlyCollection<T> items) =>
        _arr = [.. items];

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    public Block(ReadOnlySpan<T> items) =>
        _arr = [.. items];

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from an <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <param name="items">The elements to store in the array.</param>
    public Block(ImmutableArray<T> items) =>
        _arr = items.IsDefaultOrEmpty
            ? []
            // It's safe to extract the underlying array since we won't mutate it either
            : ImmutableCollectionsMarshal.AsArray(items)!;

    // Further optimizations: add single, two, three-element constructors for perf.
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
        Unsafe.ValueCollectionsMarshal.AsBlock(Array.Empty<T>());

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
            var elemType = elem.GetType();
            // If the type overrides ToString(), use that
            if (ownToString != elemType.ToString())
            {
                return ownToString;
            }
            // Otherwise if it's a collection, print it element-wise
            if (elem is ICollection coll)
            {
                return $"{GetNakedTypeName(elemType)}({coll.Count}) {PrintElems(coll)}";
            }
            if (elem is IEnumerable enumerable)
            {
                return $"{GetNakedTypeName(elemType)} {PrintElems(enumerable)}";
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
    public static Block<T> ToBlock<T>(this IEnumerable<T> items) =>
        new(items);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from a collection of items.
    /// </summary>
    /// <inheritdoc cref="ToBlock{T}(IEnumerable{T})"/>    
    public static Block<T> ToBlock<T>(this IReadOnlyCollection<T> items) =>
        items.Count == 0
            ? Block<T>.Empty
            : new(items);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from an <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <inheritdoc cref="ToBlock{T}(IEnumerable{T})"/>  
    public static Block<T> ToBlock<T>(this ImmutableArray<T> items) =>
        items.IsDefaultOrEmpty
            ? Block<T>.Empty
            : new(ImmutableCollectionsMarshal.AsArray(items)!);

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from an array or an argument list of items.
    /// </summary>
    /// <inheritdoc cref="ToBlock{T}(IEnumerable{T})"/> 
    public static Block<T> Create<T>(params T[] items) =>
        items.ToBlock();

    /// <summary>
    /// Creates a new <see cref="Block{T}"/> from a <see cref="ReadOnlySpan{T}"/> of items.
    /// </summary>
    /// <inheritdoc cref="ToBlock{T}(IEnumerable{T})"/> 
    public static Block<T> CreateRange<T>(ReadOnlySpan<T> items) =>
        items.Length == 0 
            ? Block<T>.Empty 
            : new Block<T>(items);

    /// <inheritdoc cref="ToBlock{T}(IEnumerable{T})"/> 
    public static Block<T> CreateRange<T>(IEnumerable<T> items) =>
        items.ToBlock();

    /// <inheritdoc cref="ToBlock{T}(IReadOnlyCollection{T})"/> 
    public static Block<T> CreateRange<T>(IReadOnlyCollection<T> items) =>
        items.ToBlock();

    /// <inheritdoc cref="ToBlock{T}(ImmutableArray{T})"/>
    public static Block<T> CreateRange<T>(ImmutableArray<T> items) =>
        
        items.ToBlock();
}
