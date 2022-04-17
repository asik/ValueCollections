using System;
using System.Collections.Generic;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class CreationTests
{
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
    void ConstructorFromParamsArray() =>
        Assert.Equal(
            Block.Create(1, 2, 3, 4, 5, 6),
            new Block<int>(1, 2, 3, 4, 5, 6));

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
}
