using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueCollections;

[JsonConverter(typeof(BlockJsonConverterFactory))]
public partial class Map<TKey, TValue>
{
    internal class MapJsonConverter : JsonConverter<Map<TKey, TValue>>
    {
        public override Map<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = JsonSerializer.Deserialize<ImmutableDictionary<TKey, TValue>>(ref reader, options);
            return dict!.IsEmpty
                ? Empty
                : new Map<TKey, TValue>(dict);
        }

        public override void Write(Utf8JsonWriter writer, Map<TKey, TValue> value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value._dict, options);
    }
}

class BlockJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(Map<,>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(
            typeof(Map<,>.MapJsonConverter)
                .MakeGenericType(typeToConvert.GetGenericArguments()),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null);
}

