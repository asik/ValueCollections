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

    void CreateRangeFromList() =>
        Assert.Equal(
            Block.Create(1, 2, 3),
            Block.CreateRange(new List<int> { 1, 2, 3 }));
}
