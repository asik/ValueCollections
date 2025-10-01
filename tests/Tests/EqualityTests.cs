using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ValueCollections;
using Xunit;

namespace Tests;

public class EqualityTests
{
    [Fact]
    void XUnitEquality()
    {
        // Plays well with XUnit's Assert.Equal
        var valueArray = ValueArray.Create(1, 2, 3);
        var valueArray2 = ValueArray.Create(1, 2, 3);
        // Intentionally specifying the generic type here
        Assert.Equal<ValueArray<int>>(valueArray, valueArray2);
    }

    [Fact]
    void IEquatableT()
    {
        var valueArray = ValueArray.Create(1, 2, 3);
        var valueArray2 = ValueArray.Create(1, 2, 3);
        Assert.True(valueArray.Equals(valueArray2));
    }

    [Fact]
    void ObjectEquals()
    {
        var valueArray = ValueArray.Create(1, 2, 3);
        var valueArray2 = ValueArray.Create(1, 2, 3);
        Assert.True(valueArray.Equals((object)valueArray2));
    }

    [Fact]
    void OperatorEquals()
    {
        var valueArray = ValueArray.Create(1, 2, 3);
        var valueArray2 = ValueArray.Create(1, 2, 3);
        Assert.True(valueArray == valueArray2);
    }

    [Fact]
    void OperatorNotEquals()
    {
        var valueArray = ValueArray.Create(1, 2, 3);
        var valueArray2 = ValueArray.Create(1, 2);
        Assert.True(valueArray != valueArray2);
    }

    record MyRecord(ValueArray<string> Strings);

    [Fact]
    void WorksWithRecords()
    {
        var r0 = new MyRecord(ValueArray.Create("a", "b"));
        var r1 = new MyRecord(ValueArray.Create("a", "b"));
        var r2 = new MyRecord(ValueArray.Create("b", "a"));
        Assert.True(r0.Equals(r1));
        Assert.False(r0.Equals(r2));
    }

    [Fact]
    void WorksInDictionaries()
    {
        var dict = new Dictionary<ValueArray<int>, string>
        {
            [ValueArray.Create(1, 2, 3)] = "Entry123",
            [ValueArray.Create(0, 1, 2)] = "Entry012",
            [ValueArray<int>.Empty] = "EmptyEntry",
        };

        // Keys we expect are there
        Assert.True(dict.ContainsKey(ValueArray.Create(1, 2, 3)));
        Assert.True(dict.ContainsKey(ValueArray.Create(0, 1, 2)));
        Assert.True(dict.ContainsKey(ValueArray<int>.Empty));

        // Values we expect are there
        Assert.Equal("Entry123", dict[ValueArray.Create(1, 2, 3)]);
        Assert.Equal("Entry012", dict[ValueArray.Create(0, 1, 2)]);
        Assert.Equal("EmptyEntry", dict[ValueArray<int>.Empty]);

        // Keys we don't expect aren't there
        Assert.False(dict.ContainsKey(ValueArray.Create(1, 2)));

        // We can replace an entry
        dict[ValueArray.Create(0, 1, 2)] = "Entry012_2";
        Assert.Equal("Entry012_2", dict[ValueArray.Create(0, 1, 2)]);
    }

    [Fact]
    [SuppressMessage("Assertions", "xUnit2017:Do not use Contains() to check if a value exists in a collection")]
    void WorksInHashSets()
    {
        var set = new HashSet<ValueArray<int>>
        {
            ValueArray.Create(1, 2, 3),
            ValueArray.Create(1, 2),
            ValueArray.Create(3, 2, 1),
            ValueArray.Create(1, 2)
        };
        Assert.Equal(3, set.Count);
        Assert.True(set.Contains(ValueArray.Create(1, 2)));

        // We can replace an entry
        Assert.False(set.Add(ValueArray.Create(1, 2, 3)));
        Assert.True(set.Remove(ValueArray.Create(1, 2, 3)));
        Assert.True(set.Add(ValueArray.Create(1, 2, 3)));
    }

    enum MyEnum { A, B }

    enum MyEnum2 { A, B }

    [Fact]
    void ThingsThatShouldBeEqual()
    {
        // Enums
        Assert.Equal(
            ValueArray.Create(MyEnum.A, MyEnum.B),
            ValueArray.Create(MyEnum.A, MyEnum.B));

        // strings
        Assert.Equal(
            ValueArray.Create("a", "b", "c"),
            ValueArray.Create("a", "b", "c"));

        // nulls of any kind
        Assert.Equal(
            ValueArray.Create<object?>(null, null),
            ValueArray.Create<object?>(null, null));

        // object references
        var (obj0, obj1) = (new object(), new object());
        Assert.Equal(
            ValueArray.Create(obj0, obj1),
            ValueArray.Create(obj0, obj1));

        // ValueArrays themselves
        Assert.Equal(
            ValueArray.Create(ValueArray.Create(1, 2, 3), ValueArray.Create(1, 2)),
            ValueArray.Create(ValueArray.Create(1, 2, 3), ValueArray.Create(1, 2)));
    }

    [Fact]
    void ThingsThatShouldNotBeEqual()
    {
        // Enums of different types should not be equal even with same integral values
        Assert.NotEqual(
            ValueArray.Create(MyEnum.A).Cast<object>(),
            ValueArray.Create(MyEnum2.A).Cast<object>());

        // Different number types are not equal even if they represent the same values
        Assert.NotEqual(
            ValueArray.Create(3, 4, 5).Cast<object>(),
            ValueArray.Create<uint>(3, 4, 5).Cast<object>());

        // Equality of types that don't implement IEquatable is reference equality
        Assert.NotEqual(
            ValueArray.Create(new[] { new[] { 1, 2 } }),
            ValueArray.Create(new[] { new[] { 1, 2 } }));

        Assert.NotEqual(
            ValueArray.Create(new object(), new object()),
            ValueArray.Create(new object(), new object()));
    }
}