﻿using System.Text.Json;
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

    [Fact]
    void SerializesAsAJsonArray() => 
        Assert.Equal(
            "[1,2,3]", 
            JsonSerializer.Serialize(
                Block.Create(1, 2, 3)));
}