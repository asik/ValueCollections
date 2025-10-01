using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class LinqTests
{
    readonly ValueArray<int> _valueArray = ValueArray.CreateRange(Enumerable.Range(0, 8));

    [Fact]
    void Select() =>
        Assert.Equal(
            _valueArray.Select(x => x + 1),
            Enumerable.Range(1, 8));

    [Fact]
    void Where() =>
        Assert.Equal(
            _valueArray.Where(x => x % 2 == 0),
            new[] { 0, 2, 4, 6 });

    [Fact]
    [SuppressMessage("Assertions", "xUnit2013:Do not use equality check to check for collection size.")]
    void CountEmpty() =>
        Assert.Equal(0, ValueArray<int>.Empty.Count);

    [Fact]
    void ToArray() =>
        Assert.Equal(new[] { 1, 2 }, ValueArray.Create(1, 2).ToArray());

    [Fact]
    void ToList() =>
        Assert.Equal(new List<int> { 1, 2 }, ValueArray.Create(1, 2).ToList());


    [Fact]
    void SingleThrows() =>
        Assert.Throws<InvalidOperationException>(() =>
            ValueArray.Create(1, 2).Single());

    [Fact]
    void Single() =>
        Assert.Equal(4, ValueArray.Create(4).Single());

    [Fact]
    void AsEnumerable() =>
        Assert.Equal(
            new[] { 1, 2, 3, 4 },
            ValueArray.Create(1, 2, 3, 4).AsEnumerable());

    [Fact]
    void SelectMany() =>
        Assert.Equal(
            new[] { 1, 2, 3, 4 },
            ValueArray.Create(ValueArray.Create(1, 2), ValueArray.Create(3, 4))
                .SelectMany(i => i));

    [Fact]
    void Reverse() =>
        Assert.Equal(
            new[] { 4, 3, 2, 1 },
            ValueArray.Create(1, 2, 3, 4).Reverse());

    [Fact]
    [SuppressMessage("Performance", "CA1829:Use Length/Count property instead of Count() when available")]
    void Count() =>
        Assert.Equal(3, ValueArray.Create(1, 2, 3).Count());

    // Not sure it's worth doing all of them
}
