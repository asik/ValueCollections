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
    [Fact]
    void CreateRangeFromEnumerable() =>
        Assert.Equal(
            ValueArray.Create(1, 2, 3),
            ValueArray.CreateRange(Enumerable.Range(1, 3)));

    [Fact]
    void CreateRangeFromList() =>
        Assert.Equal(
            ValueArray.Create(1, 2, 3),
            ValueArray.CreateRange(new List<int> { 1, 2, 3 }));

    [Fact]
    void ToValueArray() =>
        Assert.Equal(
            ValueArray.Create(0, 1, 2, 3, 4, 5, 6, 7),
            Enumerable.Range(0, 8).ToValueArray());

    [Fact]
    void RangesAndSlices()
    {
        var array = new[] { "0", "1", "2", "3", "4", "5" };
        var valueArray = ValueArray.Create(array);
        Assert.IsType<ValueArray<string>>(valueArray[..1]);
        Assert.Equal(array[^2], valueArray[^2]);
        Assert.Equal(array[1..^1], valueArray[1..^1]);
        Assert.Equal(array[..^0], valueArray[..^0]);
        Assert.Throws<IndexOutOfRangeException>(() => array[^0]);
    }

    [Fact]
    void CreateFromDefaultImmutableArray() =>
        Assert.Same(
            ValueArray<int>.Empty,
            ValueArray.CreateRange(default(ImmutableArray<int>)));

    [Fact]
    void CreateFromEmptyImmutableArray() =>
        Assert.Same(
            ValueArray<int>.Empty,
            ValueArray.CreateRange(ImmutableArray<int>.Empty));

    [Fact]
    void ReusesImmutableArrayInternalArray()
    {
        var immArr = ImmutableArray.Create(1, 2, 3);
        var valueArray = ValueArray.CreateRange(immArr);
        Assert.Same(
            ImmutableCollectionsMarshal.AsArray(immArr),
            ValueCollectionsMarshal.AsArray(valueArray));
    }

    [Fact]
    void EmptyLiteralCollectionIsEmpty() => 
        Assert.Equal(ValueArray<int>.Empty, []);

    [Fact]
    void EmptyLiteralCollectionIsOptimized()
    {
        // This does not test that the compiler replaces [] with ValueArray<int>.Empty
        // (it doesn't, it calls ValueArray.Create with an empty span).
        // But internally, we do a length check and return ValueArray<int>.Empty, so at least
        // we avoid creating a new instance.
        // As a result, [] is in fact less efficient than writing ValueArray<int>.Empty.
        ValueArray<int> empty = [];
        Assert.Same(ValueArray<int>.Empty, empty);
    }

    [Fact]
    void CollectionExpressionYieldsContainedItems() => 
        Assert.Equal(ValueArray.Create(2, 1, 3), [2, 1, 3]);

    [Fact]
    void CollectionExpressionSpreadElementYieldsCorrectItems()
    {
        var arr = new[] { 4, 3, 2 };
        ValueArray<int> actual = [.. arr];
        Assert.Equal(ValueArray.Create(4, 3, 2), actual);
    }

    [Fact]
    void CollectionExpressionSpreadElementComplexYieldsCorrectItems()
    {
        ValueArray<int> actual = [
            .. new[] { 4, 3, 2 }, 
            1, 
            .. new[] { 0, -1 }];
        Assert.Equal(ValueArray.Create(4, 3, 2, 1, 0, -1), actual);
    }

    [Fact]
    void CollectionExpressionSpreadElementIntoOtherCollection()
    {
        var source = new[] { 5, 4, 3 };
        ValueArray<int> arr = [.. source];
        Assert.Equal(source, arr);
    }
}
