using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class LinqTests
{
    readonly Block<int> _block = Block.CreateRange(Enumerable.Range(0, 8));

    [Fact]
    void Select() =>
        Assert.Equal(
            _block.Select(x => x + 1),
            Enumerable.Range(1, 8));

    [Fact]
    void Where() =>
        Assert.Equal(
            _block.Where(x => x % 2 == 0),
            new[] { 0, 2, 4, 6 });

    [Fact]
    [SuppressMessage("Assertions", "xUnit2013:Do not use equality check to check for collection size.")]
    void CountEmpty() =>
        Assert.Equal(0, Block<int>.Empty.Count);

    [Fact]
    void ToArray() =>
        Assert.Equal(new[] { 1, 2 }, Block.Create(1, 2).ToArray());

    [Fact]
    void ToList() =>
        Assert.Equal(new List<int> { 1, 2 }, Block.Create(1, 2).ToList());


    [Fact]
    void SingleThrows() =>
        Assert.Throws<InvalidOperationException>(() =>
            Block.Create(1, 2).Single());

    [Fact]
    void Single() =>
        Assert.Equal(4, Block.Create(4).Single());

    [Fact]
    void AsEnumerable() =>
        Assert.Equal(
            new[] { 1, 2, 3, 4 },
            Block.Create(1, 2, 3, 4).AsEnumerable());

    [Fact]
    void SelectMany() =>
        Assert.Equal(
            new[] { 1, 2, 3, 4 },
            Block.Create(Block.Create(1, 2), Block.Create(3, 4))
                .SelectMany(i => i));

    [Fact]
    void Reverse() =>
        Assert.Equal(
            new[] { 4, 3, 2, 1 },
            Block.Create(1, 2, 3, 4).Reverse());

    [Fact]
    [SuppressMessage("Performance", "CA1829:Use Length/Count property instead of Count() when available", Justification = "<Pending>")]
    void Count() =>
        Assert.Equal(3, Block.Create(1, 2, 3).Count());

    // Not sure it's worth doing all of them
}
