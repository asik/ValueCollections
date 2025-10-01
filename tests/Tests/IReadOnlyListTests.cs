using System;
using System.Collections.Generic;
using ValueCollections;
using Xunit;

namespace Tests;

public class IReadOnlyListTests
{
    readonly IReadOnlyList<string> _valueArray = ValueArray.Create("a", "b", "c");

    [Fact]
    void Index() =>
        Assert.Equal("c", _valueArray[2]);

    [Fact]
    void IndexOutOfRange() =>
        Assert.Throws<IndexOutOfRangeException>(() =>
            _valueArray[3]);

    [Fact]
    void Count() =>
        Assert.Equal(3, _valueArray.Count);

    [Fact]
    void ForEach()
    {
        var elems = new List<string>();
        foreach (var s in _valueArray)
        {
            elems.Add(s);
        }
        Assert.Equal(_valueArray, elems);
    }

    [Fact]
    void ForEach_IsStructEnumerator() =>
        Assert.True(ValueArray<int>.Empty.GetEnumerator().GetType().IsValueType);

}
