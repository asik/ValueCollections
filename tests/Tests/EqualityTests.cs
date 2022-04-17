using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class EqualityTests
{
    [Fact]
    private void XUnitEquality()
    {
        // Plays well with XUnit's Assert.Equal
        var block = Block.Create(1, 2, 3);
        var block2 = Block.Create(1, 2, 3);
        // Intentionally specifying the generic type here
        Assert.Equal<Block<int>>(block, block2);
    }

    [Fact]
    private void IEquatableT()
    {
        var block = Block.Create(1, 2, 3);
        var block2 = Block.Create(1, 2, 3);
        Assert.True(block.Equals(block2));
    }

    [Fact]
    private void IEquatableUninitialized()
    {
        Assert.True(new Block<int>().Equals(new Block<int>()));
    }

    [Fact]
    private void ObjectEquals()
    {
        var block = Block.Create(1, 2, 3);
        var block2 = Block.Create(1, 2, 3);
        Assert.True(block.Equals((object)block2));
    }

    [Fact]
    private void OperatorEquals()
    {
        var block = Block.Create(1, 2, 3);
        var block2 = Block.Create(1, 2, 3);
        Assert.True(block == block2);
    }

    [Fact]
    private void OperatorNotEquals()
    {
        var block = Block.Create(1, 2, 3);
        var block2 = Block.Create(1, 2);
        Assert.True(block != block2);
    }

    private record MyRecord(Block<string> Strings);

    [Fact]
    private void WorksWithRecords()
    {
        var r0 = new MyRecord(Block.Create("a", "b"));
        var r1 = new MyRecord(Block.Create("a", "b"));
        var r2 = new MyRecord(Block.Create("b", "a"));
        Assert.True(r0.Equals(r1));
        Assert.False(r0.Equals(r2));
    }

    [Fact]
    private void WorksInDictionaries()
    {
        var dict = new Dictionary<Block<int>, string>
        {
            [Block.Create(1, 2, 3)] = "Entry123",
            [Block.Create(0, 1, 2)] = "Entry012",
            [Block<int>.Empty] = "EmptyEntry",
        };

        // Keys we expect are there
        Assert.True(dict.ContainsKey(Block.Create(1, 2, 3)));
        Assert.True(dict.ContainsKey(Block.Create(0, 1, 2)));
        Assert.True(dict.ContainsKey(Block<int>.Empty));

        // Values we expect are there
        Assert.Equal("Entry123", dict[Block.Create(1, 2, 3)]);
        Assert.Equal("Entry012", dict[Block.Create(0, 1, 2)]);
        Assert.Equal("EmptyEntry", dict[Block<int>.Empty]);

        // Keys we don't expect aren't there
        Assert.False(dict.ContainsKey(Block.Create(1, 2)));

        // We can replace an entry
        dict[Block.Create(0, 1, 2)] = "Entry012_2";
        Assert.Equal("Entry012_2", dict[Block.Create(0, 1, 2)]);
    }

    [Fact]
    [SuppressMessage("Assertions", "xUnit2017:Do not use Contains() to check if a value exists in a collection")]
    private void WorksInHashSets()
    {
        var set = new HashSet<Block<int>>
        {
            Block.Create(1, 2, 3),
            Block.Create(1, 2),
            Block.Create(3, 2, 1),
            Block.Create(1, 2)
        };
        Assert.Equal(3, set.Count);
        Assert.True(set.Contains(Block.Create(1, 2)));

        // We can replace an entry
        Assert.False(set.Add(Block.Create(1, 2, 3)));
        Assert.True(set.Remove(Block.Create(1, 2, 3)));
        Assert.True(set.Add(Block.Create(1, 2, 3)));
    }

    private enum MyEnum { A, B }

    private enum MyEnum2 { A, B }

    [Fact]
    private void ThingsThatShouldBeEqual()
    {
        // Enums
        Assert.Equal(
            Block.Create(MyEnum.A, MyEnum.B),
            Block.Create(MyEnum.A, MyEnum.B));

        // strings
        Assert.Equal(
            Block.Create("a", "b", "c"),
            Block.Create("a", "b", "c"));

        // nulls of any kind
        Assert.Equal(
            Block.Create<object?>(null, null),
            Block.Create<object?>(null, null));

        // object references
        var (obj0, obj1) = (new object(), new object());
        Assert.Equal(
            Block.Create(obj0, obj1),
            Block.Create(obj0, obj1));

        // Blocks themselves
        Assert.Equal(
            Block.Create(Block.Create(1, 2, 3), Block.Create(1, 2)),
            Block.Create(Block.Create(1, 2, 3), Block.Create(1, 2)));
    }

    [Fact]
    private void ThingsThatShouldNotBeEqual()
    {
        // Enums of different types should not be equal even with same integral values
        Assert.NotEqual(
            Block.Create(MyEnum.A).Cast<object>(),
            Block.Create(MyEnum2.A).Cast<object>());

        // Different number types are not equal even if they represent the same values
        Assert.NotEqual(
            Block.Create(3, 4, 5).Cast<object>(),
            Block.Create<uint>(3, 4, 5).Cast<object>());

        // Equality of types that don't implement IEquatable is reference equality
        Assert.NotEqual(
            Block.Create(new[] { new[] { 1, 2 } }),
            Block.Create(new[] { new[] { 1, 2 } }));

        Assert.NotEqual(
            Block.Create(new object(), new object()),
            Block.Create(new object(), new object()));
    }



    //[Fact]
    //void CompareTests()
    //{
    //    // Sanity
    //    Assert.True(Block.Create(1) > Block<int>.Empty);
    //    Assert.False(Block.Create(1) < Block<int>.Empty);
    //    Assert.True(Block<int>.Empty < Block.Create(1));
    //    Assert.False(Block<int>.Empty > Block.Create(1));

    //    // Size
    //    Assert.True(Block.Create(0, 0) > Block.Create(0));
    //    Assert.True(Block.Create(1) < Block.Create(1, 1));

    //    // Same size - First different element
    //}
}