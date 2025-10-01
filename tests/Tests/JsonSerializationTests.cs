using System.Text.Json;
using ValueCollections;
using Xunit;

namespace Tests;

public class JsonSerializationTests
{

    [Fact]
    void CanSerializeAndDeserializeBackOutOfTheBox()
    {
        var original = ValueArray.Create(1, 2, 3);
        var serialized = JsonSerializer.Serialize(original);
        var deserialized =
            JsonSerializer.Deserialize<ValueArray<int>>(serialized);

        Assert.Equal(original, deserialized);
    }

    record InnerType(string A);
    record ComplexType(string A, int B, ValueArray<InnerType> Inner);

    [Fact]
    void CanSerializeAndDeserializeComplexType()
    {
        var original = new ComplexType(
            A: "abc",
            B: 3,
            Inner: ValueArray.Create(new InnerType("w"), new InnerType("h")));

        var serialized = JsonSerializer.Serialize(original);
        var deserialized =
            JsonSerializer.Deserialize<ComplexType>(serialized);

        Assert.Equal(original, deserialized);
    }

    [Fact]
    void CanSerializeAndDeserializeEmptyArray()
    {
        var serialized = JsonSerializer.Serialize(ValueArray<int>.Empty);
        var deserialized =
            JsonSerializer.Deserialize<ValueArray<int>>(serialized);

        Assert.Same(ValueArray<int>.Empty, deserialized);
    }

    [Fact]
    void SerializesAsAJsonArray() =>
        Assert.Equal(
            "[1,2,3]",
            JsonSerializer.Serialize(
                ValueArray.Create(1, 2, 3)));

    [Fact]
    void SerializesEmptyAsAnEmptyArray() =>
        Assert.Equal(
            "[]",
            JsonSerializer.Serialize(ValueArray<int>.Empty));
}
