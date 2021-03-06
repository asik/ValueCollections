using System;
using System.Collections.Immutable;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;
public class AppendTests
{
    [Fact]
    void Append()
    {
        // Append array
        Assert.Equal(
            Block.Create(1, 2, 3, 1, 2, 3),
            Block.Create(1, 2, 3).Append(new[] { 1, 2, 3 }));

        // Append single value
        Assert.Equal(
            Block.Create(1, 2, 3, 2),
            Block.Create(1, 2, 3).Append(2));

        // Append collection of collections resolves to the right .Append overload
        Assert.Equal(
            Block.Create(1, 2, 3, 4),
            Block.Create(new[] { 1 }, new[] { 2 })
                .Append(new[] { new[] { 3 }, new[] { 4 } })
                .SelectMany(arr => arr)
                .ToBlock());

        // Append string to a Block<char>
        Assert.Equal(
            Block.Create('a', 'b', 'c', 'd'),
            Block.Create('a', 'b').Append("cd"));

        // Append - result is empty - Block overload
        Assert.Same(
            Block<int>.Empty,
            Block<int>.Empty.Append(Block<int>.Empty));

        // Append - result is empty - IEnumerable overload
        Assert.Same(
            Block<int>.Empty,
            Block<int>.Empty.Append(Array.Empty<int>()));
    }

    [Fact]
    void Insert()
    {
        // Insert one
        Assert.Equal(
            Block.Create("a", "b", "c"),
            Block.Create("a", "c").Insert(1, "b"));

        // Insert multiple
        Assert.Equal(
            Block.Create("a", "b", "c", "d"),
            Block.Create("a", "d").Insert(1, new[] { "b", "c" }));

        // Insert string in char array
        Assert.Equal(
            Block.Create('a', 'b', 'c', 'd'),
            Block.Create('a', 'd').Insert(1, "bc"));

        // Insert into empty
        Assert.Equal(
            Block.Create(1, 2, 3, 4),
            Block<int>.Empty.Insert(0, new[] { 1, 2, 3, 4 }));

        // Result is empty - IEnumerable overload
        Assert.Same(
            Block<int>.Empty,
            Block<int>.Empty.Insert(0, Array.Empty<int>()));

        // Result is empty - Block overload
        Assert.Same(
            Block<int>.Empty,
            Block<int>.Empty.Insert(0, Block<int>.Empty));

        // Exception
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Block<int>.Empty.Insert(1, 34));
    }

    [Fact]
    void Remove()
    {
        //Assert.Equal(
        //    Block.Create("a", "c"),
        //    Block.Create("a", "b", "c").RemoveFirst("b"));

        //Assert.Equal(
        //    Block.Create("a", "b", "b", "c", "b"),
        //    Block.Create("b", "a", "b", "b", "c", "b").RemoveFirst("b"));

        //Assert.Equal(
        //    Block.Create("a", "d", "b", "b", "b"),
        //    Block.Create("b", "a", "d", "b", "b", "c", "b").RemoveFirst(new[] { "b", "c" }));

        Assert.Equal(
            Block.Create("a"),
            Block.Create("a", "b").RemoveAt(1));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Block.Create("a").RemoveAt(1));

        Assert.Same(
            Block<int>.Empty,
            Block.Create(1).RemoveAt(0));
    }

}
