using System;
using System.Collections.Generic;
using ValueCollections;
using Xunit;

namespace Tests;

public class IReadOnlyListTests
{
    readonly IReadOnlyList<string> _block = Block.Create("a", "b", "c");

    [Fact]
    void Index() =>
        Assert.Equal("c", _block[2]);

    [Fact]
    void IndexOutOfRange() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            _block[3]);

    [Fact]
    void Count() =>
        Assert.Equal(3, _block.Count);

    [Fact]
    void ForEach()
    {
        var elems = new List<string>();
        foreach (var s in _block)
        {
            elems.Add(s);
        }
        Assert.Equal(_block, elems);
    }

    [Fact]
    void ForEach_IsStructEnumerator() =>
        Assert.True(Block<int>.Empty.GetEnumerator().GetType().IsValueType);

}
