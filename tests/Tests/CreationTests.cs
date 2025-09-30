using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using ValueCollections;
using ValueCollections.Unsafe;
using Xunit;

namespace Tests;

public class CreationTests
{
    // TODO should we just make all constructors internal?

    [Fact]
    void ConstructorFromEnumerable() =>
        Assert.Equal(
            Block.Create(1, 2, 3),
            new Block<int>(Enumerable.Range(1, 3)));

    [Fact]
    void ConstructorFromList() =>
        Assert.Equal(
            Block.Create(1, 2, 3),
            new Block<int>(new List<int> { 1, 2, 3 }));

    [Fact]
    void CreateRangeFromEnumerable() =>
        Assert.Equal(
            Block.Create(1, 2, 3),
            Block.CreateRange(Enumerable.Range(1, 3)));

    [Fact]
    void CreateRangeFromList() =>
        Assert.Equal(
            Block.Create(1, 2, 3),
            Block.CreateRange(new List<int> { 1, 2, 3 }));

    [Fact]
    void ToBlock() =>
        Assert.Equal(
            Block.Create(0, 1, 2, 3, 4, 5, 6, 7),
            Enumerable.Range(0, 8).ToBlock());

    [Fact]
    void RangesAndSlices()
    {
        var array = new[] { "0", "1", "2", "3", "4", "5" };
        var block = Block.Create(array);
        Assert.IsType<Block<string>>(block[..1]);
        Assert.Equal(array[^2], block[^2]);
        Assert.Equal(array[1..^1], block[1..^1]);
        Assert.Equal(array[..^0], block[..^0]);
        Assert.Throws<IndexOutOfRangeException>(() => array[^0]);
    }

    [Fact]
    void CreateFromDefaultImmutableArray() =>
        Assert.Same(
            Block<int>.Empty,
            Block.CreateRange(default(ImmutableArray<int>)));

    [Fact]
    void CreateFromEmptyImmutableArray() =>
        Assert.Same(
            Block<int>.Empty,
            Block.CreateRange(ImmutableArray<int>.Empty));

    [Fact]
    void ReusesImmutableArrayInternalArray()
    {
        var immArr = ImmutableArray.Create(1, 2, 3);
        var block = Block.CreateRange(immArr);
        Assert.Same(
            ImmutableCollectionsMarshal.AsArray(immArr),
            ValueCollectionsMarshal.AsArray(block));
    }

    [Fact]
    void EmptyLiteralCollectionIsEmpty() => 
        Assert.Equal(Block<int>.Empty, []);

    [Fact]
    void EmptyLiteralCollectionIsOptimized()
    {
        // This does not test that the compiler replaces [] with Block<int>.Empty
        // (it doesn't, it calls Block.Create with an empty span).
        // But internally, we do a length check and return Block<int>.Empty, so at least
        // we avoid creating a new instance.
        // As a result, [] is in fact less efficient than writing Block<int>.Empty.
        Block<int> empty = [];
        Assert.Same(Block<int>.Empty, empty);
    }

    [Fact]
    void CollectionExpressionYieldsContainedItems() => 
        Assert.Equal(Block.Create(2, 1, 3), [2, 1, 3]);

    [Fact]
    void CollectionExpressionSpreadElementYieldsCorrectItems()
    {
        var arr = new[] { 4, 3, 2 };
        Block<int> actual = [.. arr];
        Assert.Equal(Block.Create(4, 3, 2), actual);
    }

    [Fact]
    void CollectionExpressionSpreadElementComplexYieldsCorrectItems()
    {
        Block<int> actual = [
            .. new[] { 4, 3, 2 }, 
            1, 
            .. new[] { 0, -1 }];
        Assert.Equal(Block.Create(4, 3, 2, 1, 0, -1), actual);
    }

    [Fact]
    void CollectionExpressionSpreadElementIntoOtherCollection()
    {
        var source = new[] { 5, 4, 3 };
        Block<int> arr = [.. source];
        Assert.Equal(source, arr);
    }
}
