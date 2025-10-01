using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ValueCollections;

[JsonConverter(typeof(ValueArrayJsonConverterFactory))]
public partial class ValueArray<T>
{
    internal class ValueArrayJsonConverter : JsonConverter<ValueArray<T>>
    {
        public override ValueArray<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            // We need to do override deserialization because otherwise JsonSerializer tries to deserialize ValueArray<T> as a collection,
            // and the only generic path it has for collections is through ICollection<T>, calling ICollection<T>.Add
            // for each element. But implementing that interface would make our type mutable!

            // It's possible we won't need to this in the future and will be able to rely on JsonConverter calling our constructor, maybe?
            // https://github.com/dotnet/runtime/issues/63791
            JsonSerializer
            // TODO why not just deserialize to T[] and then unsafe wrap?
                .Deserialize<ImmutableArray<T>>(ref reader, options)
                .ToValueArray();

        public override void Write(Utf8JsonWriter writer, ValueArray<T> value, JsonSerializerOptions options) =>
            // We don't actually want to override serialization, but when we inherit from JsonConverter we have to.
            // Just tell JsonSerializer to treat is as an IEnumerable<T> and it'll convert it to a JSON array.
            JsonSerializer.Serialize(writer, value._arr, options);
    }
}

class ValueArrayJsonConverterFactory : JsonConverterFactory
{
    // If I try and stick [JsonConverter(typeof(ValueArrayJsonConverter<>)] on ValueArray<T> directly,
    // that just doesn't work, System.Text.Json doesn't like it. The pattern for generic types
    // is to go through a factory. Luckily there's good documentation on how to do that,
    // and luckily I can put [JsonConverter(typeof(ValueArrayJsonConverterFactory)] on ValueArray<T>
    // so everything automagically works for users.
    // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-round-trip-for-stackt

    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType
        && typeToConvert.GetGenericTypeDefinition() == typeof(ValueArray<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (JsonConverter)Activator.CreateInstance(
            typeof(ValueArray<>.ValueArrayJsonConverter)
                .MakeGenericType(typeToConvert.GetGenericArguments()),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null);
}
