using System.Text.Json;
using ValueCollections;
using Xunit;

namespace Tests;

public class JsonSerializationTests
{

    [Fact]
    void CanSerializeAndDeserializeBackOutOfTheBox()
    {
        var original = Block.Create(1, 2, 3);
        var serialized = JsonSerializer.Serialize(original);
        var deserialized =
            JsonSerializer.Deserialize<Block<int>>(serialized);

        Assert.Equal(original, deserialized);
    }

    record InnerType(string A);
    record ComplexType(string A, int B, Block<InnerType> Inner);

    [Fact]
    void CanSerializeAndDeserializeComplexType()
    {
        var original = new ComplexType(
            A: "abc",
            B: 3,
            Inner: new Block<InnerType>(new InnerType("w"), new InnerType("h")));

        var serialized = JsonSerializer.Serialize(original);
        var deserialized =
            JsonSerializer.Deserialize<ComplexType>(serialized);

        Assert.Equal(original, deserialized);
    }

    [Fact]
    void CanSerializeAndDeserializeEmptyArray()
    {
        var serialized = JsonSerializer.Serialize(Block<int>.Empty);
        var deserialized =
            JsonSerializer.Deserialize<Block<int>>(serialized);

        Assert.Equal(Block<int>.Empty, deserialized);
    }

    [Fact]
    void SerializesAsAJsonArray() =>
        Assert.Equal(
            "[1,2,3]",
            JsonSerializer.Serialize(
                Block.Create(1, 2, 3)));

    [Fact]
    void SerializesEmptyAsAnEmptyArray() =>
        Assert.Equal(
            "[]",
            JsonSerializer.Serialize(Block<int>.Empty));
}
