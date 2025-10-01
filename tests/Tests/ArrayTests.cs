using System;
using System.Collections.Generic;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;


public class ArrayTests
{
    [Fact]
    void LengthEmpty() =>
        Assert.Equal(0, ValueArray<int>.Empty.Length);

    [Fact]
    void EmptyIsEmpty() =>
        Assert.Equal(Array.Empty<int>(), ValueArray<int>.Empty);

    [Fact]
    void EmptyIsSingleton() =>
        Assert.Same(ValueArray<int>.Empty, ValueArray<int>.Empty);

    [Fact]
    void FindIndexi_NotFound()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = source.Select((item, i) => (item, i));
        var actual = new List<(string, int)>();
        var idx = source.FindIndex((item, i) =>
        {
            actual.Add((item, i));
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    [Fact]
    void FindIndexi_Found()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = new[] { ("a", 0), ("b", 1) };
        var actual = new List<(string, int)>();
        var idx = source.FindIndex((item, i) =>
        {
            actual.Add((item, i));
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(1, idx);
    }

    [Fact]
    void FindLastIndexi_NotFound()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = new[] { ("c", 3), ("b", 2), ("b", 1), ("a", 0) };
        var actual = new List<(string, int)>();
        var idx = source.FindLastIndex((item, i) =>
        {
            actual.Add((item, i));
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    [Fact]
    void FindLastIndexi_Found()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = new[] { ("c", 3), ("b", 2) };
        var actual = new List<(string, int)>();
        var idx = source.FindLastIndex((item, i) =>
        {
            actual.Add((item, i));
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(2, idx);
    }

    [Fact]
    void FindIndex_NotFound()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = source.ToArray();
        var actual = new List<string>();
        var idx = source.FindIndex(item =>
        {
            actual.Add(item);
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    [Fact]
    void FindIndex_Found()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = new[] { "a", "b" };
        var actual = new List<string>();
        var idx = source.FindIndex(item =>
        {
            actual.Add(item);
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(1, idx);
    }

    [Fact]
    void FindLastIndex_NotFound()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = new[] { "c", "b", "b", "a" };
        var actual = new List<string>();
        var idx = source.FindLastIndex(item =>
        {
            actual.Add(item);
            return false;
        });
        Assert.Equal(expected, actual);
        Assert.Equal(-1, idx);
    }

    [Fact]
    void FindLastIndex_Found()
    {
        var source = ValueArray.Create("a", "b", "b", "c");
        var expected = new[] { "c", "b" };
        var actual = new List<string>();
        var idx = source.FindLastIndex(item =>
        {
            actual.Add(item);
            return item == "b";
        });
        Assert.Equal(expected, actual);
        Assert.Equal(2, idx);
    }
}
