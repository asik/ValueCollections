using System;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class TransformTests
{
    [Fact]
    void AppendArray() =>
        Assert.Equal(
            Block.Create(1, 2, 3, 1, 2, 3),
            Block.Create(1, 2, 3).Append(new[] { 1, 2, 3 }));

    [Fact]
    void AppendSingleValue() =>
        Assert.Equal(
            Block.Create(1, 2, 3, 2),
            Block.Create(1, 2, 3).Append(2));

    [Fact]
    void AppendSingleValueToEmptyBlock() =>
        Assert.Equal(
            Block.Create(2),
            Block<int>.Empty.Append(2));


    [Fact]
    void AppendCollectionOfCollections() =>
        // Resolves to the right .Append overload
        Assert.Equal(
            Block.Create(1, 2, 3, 4),
            Block.Create(new[] { 1 }, new[] { 2 })
                .Append(new[] { new[] { 3 }, new[] { 4 } })
                .SelectMany(arr => arr)
                .ToBlock());

    [Fact]
    void AppendStringToABlockOfChar() => 
        Assert.Equal(
            Block.Create('a', 'b', 'c', 'd'),
            Block.Create('a', 'b').Append("cd"));

    [Fact]
    void AppendABlockToAnotherBlock() =>
        Assert.Equal(
            Block.Create(1, 2, 3, 4),
            Block.Create(1, 2).Append(Block.Create(3, 4)));

    [Fact]
    void AppendABlockToAnEmptyBlock() =>
        Assert.Equal(
            Block.Create(3, 4),
            Block<int>.Empty.Append(Block.Create(3, 4)));

    [Fact]
    void AppendAnEmptyBlockToABlock() =>
        Assert.Equal(
            Block.Create(1, 2),
            Block.Create(1, 2).Append(Block<int>.Empty));

    [Fact]
    void AppendAnEmptyBlockToABlock_Optimized() =>
        Assert.Same(
            Block<int>.Empty,
            Block<int>.Empty.Append(Block<int>.Empty));

    [Fact]
    void InsertOneItemAtTheBeginning() =>
        Assert.Equal(
            Block.Create("a", "b", "c"),
            Block.Create("b", "c").Insert(0, "a"));
    [Fact]
    void InsertOneItemInTheMiddle() =>
        Assert.Equal(
            Block.Create("a", "b", "c"),
            Block.Create("a", "c").Insert(1, "b"));

    [Fact]
    void InsertOneItemAtTheEnd() =>
        Assert.Equal(
            Block.Create("a", "b", "c"),
            Block.Create("a", "b").Insert(2, "c"));

    [Fact]
    void InsertRangeAtTheBeginning() =>
        Assert.Equal(
            Block.Create("a", "b", "c", "d"),
            Block.Create("c", "d").Insert(0, new[] { "a", "b" }));

    [Fact]
    void InsertRangeInTheMiddle() =>
        Assert.Equal(
            Block.Create("a", "b", "c", "d"),
            Block.Create("a", "d").Insert(1, new[] { "b", "c" }));

    [Fact]
    void InsertRangeAtTheEnd() =>
        Assert.Equal(
            Block.Create("a", "b", "c", "d"),
            Block.Create("a", "b").Insert(2, new[] { "c", "d" }));

    void InsertStringInCharArray() =>
        Assert.Equal(
            Block.Create('a', 'b', 'c', 'd'),
            Block.Create('a', 'd').Insert(1, "bc"));

    void InsertIntoAnEmptyBlock() =>
        Assert.Equal(
            Block.Create(1, 2, 3, 4),
            Block<int>.Empty.Insert(0, new[] { 1, 2, 3, 4 }));

    [Fact]
    void InsertEmptyReturnsSameInstance() =>
        Assert.Same(
            Block<int>.Empty,
            Block<int>.Empty.Insert(0, Block<int>.Empty));


    [Fact]
    void InsertBeyondEndThrows() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            Block.Create(0, 1, 2).Insert(4, 3));

    [Fact]
    void InsertAtNegativeIndexThrows() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            Block.Create(0, 1, 2).Insert(-1, -1));

    [Fact]
    void RemoveAtBeginning() =>
        Assert.Equal(
            Block.Create("b", "c"),
            Block.Create("a", "b", "c").RemoveAt(0));

    [Fact]
    void RemoveInMiddle() =>
        Assert.Equal(
            Block.Create("a", "c"),
            Block.Create("a", "b", "c").RemoveAt(1));

    [Fact]
    void RemoveAtEnd() =>
        Assert.Equal(
            Block.Create("a", "b"),
            Block.Create("a", "b", "c").RemoveAt(2));

    [Fact]
    void RemoveBeyondEndThrows() =>
        Assert.Throws<IndexOutOfRangeException>(() => 
            Block.Create(1, 2, 3).RemoveAt(3));

    [Fact]
    void RemoveAtNegativeIndexThrows() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            Block.Create(1, 2, 3).RemoveAt(-1));

    [Fact]
    void RemoveLastElementReturnsEmptyInstance() =>
        Assert.Same(
            Block<int>.Empty,
            Block.Create(1).RemoveAt(0));

    [Fact]
    void SetItemOnEmptyArrayThrows() =>
        Assert.Throws<IndexOutOfRangeException>(() => 
            Block<string>.Empty.SetItem(0, "a"));

    [Fact]
    void SetItemOnNegativeIndexThrows() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            Block.Create(1, 2, 3).SetItem(-1, 94));

    [Fact]
    void SetItemBeyondEndIndexThrows() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            Block.Create(1, 2, 3).SetItem(3, 94));

    [Fact]
    void SetItemReplacesItemInMiddle()
    {
        var original = Block.Create(1, 2, 3);
        var edited = original.SetItem(1, -2);
        Assert.Equal(Block.Create(1, -2, 3), edited);
    }

    [Fact]
    void SetItemReplacesItemAtBeginning()
    {
        var original = Block.Create(1, 2, 3);
        var edited = original.SetItem(0, -2);
        Assert.Equal(Block.Create(-2, 2, 3), edited);
    }

    [Fact]
    void SetItemReplacesItemAtEnd()
    {
        var original = Block.Create(1, 2, 3);
        var edited = original.SetItem(2, -2);
        Assert.Equal(Block.Create(1, 2, -2), edited);
    }
}
