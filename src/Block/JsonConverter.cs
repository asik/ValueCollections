using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueCollections.Json;

class BlockJsonConverter<T> : JsonConverter<Block<T>>
{
    public override Block<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // We need to do override deserialization because otherwise JsonSerializer tries to deserialize Block<T> as a collection,
        // and the only generic path it has for collections is through ICollection<T>, calling ICollection<T>.Add
        // for each element. But implementing that interface would make our type mutable!

        // It's possible we won't need to this in the future and will be able to rely on JsonConverter calling our constructor, maybe?
        // https://github.com/dotnet/runtime/issues/63791
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }
        reader.Read();

        var list = new List<T>();

        while (reader.TokenType != JsonTokenType.EndArray)
        {
            list.Add(JsonSerializer.Deserialize<T>(ref reader, options));
            reader.Read();
        }

        return list.Count > 0
            ? new Block<T>(list)
            : Block<T>.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Block<T> value, JsonSerializerOptions options) =>
        // We don't actually want to override serialization, but when we inherit from JsonConverter we have to.
        // Just tell JsonSerializer to treat is as an IEnumerable<T> and it'll convert it to a JSON array.
        JsonSerializer.Serialize(writer, value as IEnumerable<T>, options);
}

class BlockJsonConverterFactory : JsonConverterFactory
{
    // If I try and stick [JsonConverter(typeof(BlockJsonConverter<>)] on Block<T> directly,
    // that just doesn't work, System.Text.Json doesn't like it. The pattern for generic types
    // is to go through a factory. Luckily there's good documentation on how to do that,
    // and luckily I can put [JsonConverter(typeof(BlockJsonConverterFactory)] on Block<T>
    // so everything automagically works for users.
    // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-round-trip-for-stackt

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(Block<>);

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(
            typeof(BlockJsonConverter<>)
                .MakeGenericType(new[] { typeToConvert.GetGenericArguments()[0] }),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null);
}
