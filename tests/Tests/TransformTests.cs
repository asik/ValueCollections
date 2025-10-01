using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class TransformTests
{
    [Fact]
    void AddArray() =>
        Assert.Equal(
            ValueArray.Create(1, 2, 3, 1, 2, 3),
            ValueArray.Create(1, 2, 3).AddRange(new[] { 1, 2, 3 }));

    [Fact]
    void AddSingleValue() =>
        Assert.Equal(
            ValueArray.Create(1, 2, 3, 2),
            ValueArray.Create(1, 2, 3).Add(2));

    [Fact]
    void AddSingleValueToEmptyValueArray() =>
        Assert.Equal(
            ValueArray.Create(2),
            ValueArray<int>.Empty.Add(2));


    [Fact]
    void AddCollectionOfCollections() =>
        Assert.Equal(
            ValueArray.Create(1, 2, 3, 4),
            ValueArray.Create(new[] { 1 }, new[] { 2 })
                .AddRange(new[] { new[] { 3 }, new[] { 4 } })
                .SelectMany(arr => arr)
                .ToValueArray());

    [Fact]
    void AddStringToAValueArrayOfChar() => 
        Assert.Equal(
            ValueArray.Create('a', 'b', 'c', 'd'),
            ValueArray.Create('a', 'b').AddRange("cd"));

    [Fact]
    void AddAValueArrayToAnotherValueArray() =>
        Assert.Equal(
            ValueArray.Create(1, 2, 3, 4),
            ValueArray.Create(1, 2).AddRange(ValueArray.Create(3, 4)));

    [Fact]
    void AddAValueArrayToAnEmptyValueArray() =>
        Assert.Equal(
            ValueArray.Create(3, 4),
            ValueArray<int>.Empty.AddRange(ValueArray.Create(3, 4)));

    [Fact]
    void AddAnEmptyValueArrayToAValueArray() =>
        Assert.Equal(
            ValueArray.Create(1, 2),
            ValueArray.Create(1, 2).AddRange(ValueArray<int>.Empty));

    [Fact]
    void AddAnEmptyValueArrayToAValueArray_Optimized() =>
        Assert.Same(
            ValueArray<int>.Empty,
            ValueArray<int>.Empty.AddRange(ValueArray<int>.Empty));

    [Fact]
    void InsertOneItemAtTheBeginning() =>
        Assert.Equal(
            ValueArray.Create("a", "b", "c"),
            ValueArray.Create("b", "c").Insert(0, "a"));
    [Fact]
    void InsertOneItemInTheMiddle() =>
        Assert.Equal(
            ValueArray.Create("a", "b", "c"),
            ValueArray.Create("a", "c").Insert(1, "b"));

    [Fact]
    void InsertOneItemAtTheEnd() =>
        Assert.Equal(
            ValueArray.Create("a", "b", "c"),
            ValueArray.Create("a", "b").Insert(2, "c"));

    [Fact]
    void InsertRangeAtTheBeginning() =>
        Assert.Equal(
            ValueArray.Create("a", "b", "c", "d"),
            ValueArray.Create("c", "d").InsertRange(0, new[] { "a", "b" }));

    [Fact]
    void InsertRangeInTheMiddle() =>
        Assert.Equal(
            ValueArray.Create("a", "b", "c", "d"),
            ValueArray.Create("a", "d").InsertRange(1, new[] { "b", "c" }));

    [Fact]
    void InsertRangeAtTheEnd() =>
        Assert.Equal(
            ValueArray.Create("a", "b", "c", "d"),
            ValueArray.Create("a", "b").InsertRange(2, new[] { "c", "d" }));

    void InsertStringInCharArray() =>
        Assert.Equal(
            ValueArray.Create('a', 'b', 'c', 'd'),
            ValueArray.Create('a', 'd').InsertRange(1, "bc"));

    void InsertIntoAnEmptyValueArray() =>
        Assert.Equal(
            ValueArray.Create(1, 2, 3, 4),
            ValueArray<int>.Empty.InsertRange(0, new[] { 1, 2, 3, 4 }));

    [Fact]
    void InsertEmptyReturnsSameInstance() =>
        Assert.Same(
            ValueArray<int>.Empty,
            ValueArray<int>.Empty.InsertRange(0, ValueArray<int>.Empty));


    [Fact]
    void InsertBeyondEndThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ValueArray.Create(0, 1, 2).Insert(4, 3));

    [Fact]
    void InsertAtNegativeIndexThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ValueArray.Create(0, 1, 2).Insert(-1, -1));

    [Fact]
    void RemoveAtBeginning() =>
        Assert.Equal(
            ValueArray.Create("b", "c"),
            ValueArray.Create("a", "b", "c").RemoveAt(0));

    [Fact]
    void RemoveInMiddle() =>
        Assert.Equal(
            ValueArray.Create("a", "c"),
            ValueArray.Create("a", "b", "c").RemoveAt(1));

    [Fact]
    void RemoveAtEnd() =>
        Assert.Equal(
            ValueArray.Create("a", "b"),
            ValueArray.Create("a", "b", "c").RemoveAt(2));

    [Fact]
    void RemoveBeyondEndThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            ValueArray.Create(1, 2, 3).RemoveAt(3));

    [Fact]
    void RemoveAtNegativeIndexThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ValueArray.Create(1, 2, 3).RemoveAt(-1));

    [Fact]
    void RemoveLastElementReturnsEmptyInstance() =>
        Assert.Same(
            ValueArray<int>.Empty,
            ValueArray.Create(1).RemoveAt(0));

    [Fact]
    void SetItemOnEmptyArrayThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => 
            ValueArray<string>.Empty.SetItem(0, "a"));

    [Fact]
    void SetItemOnNegativeIndexThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ValueArray.Create(1, 2, 3).SetItem(-1, 94));

    [Fact]
    void SetItemBeyondEndIndexThrows() =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ValueArray.Create(1, 2, 3).SetItem(3, 94));

    [Fact]
    void SetItemReplacesItemInMiddle()
    {
        var original = ValueArray.Create(1, 2, 3);
        var edited = original.SetItem(1, -2);
        Assert.Equal(ValueArray.Create(1, -2, 3), edited);
    }

    [Fact]
    void SetItemReplacesItemAtBeginning()
    {
        var original = ValueArray.Create(1, 2, 3);
        var edited = original.SetItem(0, -2);
        Assert.Equal(ValueArray.Create(-2, 2, 3), edited);
    }

    [Fact]
    void SetItemReplacesItemAtEnd()
    {
        var original = ValueArray.Create(1, 2, 3);
        var edited = original.SetItem(2, -2);
        Assert.Equal(ValueArray.Create(1, 2, -2), edited);
    }

    [Fact]
    void SortDefaultSortsAscending()
    {
        var original = ValueArray.Create(4, 2, 6, 4);
        var sorted = original.Sort();
        Assert.Equal(ValueArray.Create(2, 4, 4, 6), sorted);
    }

    [Fact]
    void SortWithCustomComparer()
    {
        var comparer = new CustomComparer<int>((a, b) => b.CompareTo(a));
        var original = ValueArray.Create(4, 2, 6, 4);
        var sorted = original.Sort(comparer);
        Assert.Equal(ValueArray.Create(6, 4, 4, 2), sorted);
    }

    [Fact]
    void SortEmptyReturnsSameInstance() =>
        Assert.Same(
            ValueArray<int>.Empty,
            ValueArray<int>.Empty.Sort());
}

class CustomComparer<T>(Func<T, T, int> compare) : IComparer<T>
{
    public int Compare(T? x, T? y) => compare(x!, y!);
}


