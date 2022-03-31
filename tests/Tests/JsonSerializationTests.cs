using System.Collections.Immutable;
using System.Text.Json;
using ValueCollections;
using Xunit;

namespace Tests;
public class JsonSerializationTests
{
    [Fact]
    void Try()
    {
        var original = Block.Create(1, 2, 3);
        var serialized = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<Block<int>>(serialized);

        Assert.Equal(original, deserialized);
    }
    [Fact]
    void TryIAR()
    {
        var original = ImmutableArray.Create(1, 2, 3);
        var serialized = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<ImmutableArray<int>>(serialized);

        Assert.Equal(original.ToBlock(), deserialized.ToBlock());
    }
}
