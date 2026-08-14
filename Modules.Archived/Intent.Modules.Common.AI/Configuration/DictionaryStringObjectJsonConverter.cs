using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Configuration
{
    /// <summary>
    /// A custom <see cref="JsonConverter{T}"/> for serializing and deserializing <see cref="Dictionary{TKey, TValue}"/>
    /// with <c>string</c> keys and <c>object</c> values, supporting nested dictionaries and arrays.
    /// </summary>
    public sealed class DictionaryStringObjectJsonConverter
        : JsonConverter<Dictionary<string, object>>
    {
        /// <summary>
        /// Reads and converts the JSON to a <see cref="Dictionary{TKey, TValue}"/> with <c>string</c> keys and <c>object</c> values.
        /// Supports nested objects and arrays.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">Options to use for deserialization.</param>
        /// <returns>The deserialized dictionary.</returns>
        /// <exception cref="JsonException">Thrown if the JSON is not in the expected format.</exception>
        public override Dictionary<string, object> Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dict;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                var propName = reader.GetString()!;
                reader.Read();
                dict[propName] = ReadValue(ref reader, options);
            }

            return dict;
        }

        /// <summary>
        /// Reads a value from the JSON reader, handling objects, arrays, strings, numbers, booleans, and nulls.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="options">Options to use for deserialization.</param>
        /// <returns>The deserialized value.</returns>
        /// <exception cref="JsonException">Thrown if the JSON token is not supported.</exception>
        private static object? ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    // recurse using this same converter
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);

                case JsonTokenType.StartArray:
                    {
                        var list = new List<object?>();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray) break;
                            list.Add(ReadValue(ref reader, options));
                        }
                        return list;
                    }

                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out var dt)) return dt; // optional
                    return reader.GetString();

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var l)) return l;
                    if (reader.TryGetDouble(out var d)) return d;
                    return reader.GetDecimal();

                case JsonTokenType.True: return true;
                case JsonTokenType.False: return false;
                case JsonTokenType.Null: return null;
            }

            throw new JsonException();
        }

        /// <summary>
        /// Writes a <see cref="Dictionary{TKey, TValue}"/> with <c>string</c> keys and <c>object</c> values to JSON.
        /// Supports nested objects and arrays.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The dictionary value to write.</param>
        /// <param name="options">Options to use for serialization.</param>
        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kv in value)
            {
                writer.WritePropertyName(kv.Key);
                WriteValue(writer, kv.Value, options);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes an object value to JSON, handling primitive types, dictionaries, and arrays.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="options">Options to use for serialization.</param>
        private static void WriteValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (value is null) { writer.WriteNullValue(); return; }

            switch (value)
            {
                case string s: writer.WriteStringValue(s); return;
                case bool b: writer.WriteBooleanValue(b); return;
                case int i: writer.WriteNumberValue(i); return;
                case long l: writer.WriteNumberValue(l); return;
                case double d: writer.WriteNumberValue(d); return;
                case decimal m: writer.WriteNumberValue(m); return;
                case DateTime dt: writer.WriteStringValue(dt); return;

                case Dictionary<string, object> dict:
                    JsonSerializer.Serialize(writer, dict, options); return;

                case IEnumerable<object?> seq when value is not string:
                    writer.WriteStartArray();
                    foreach (var item in seq) WriteValue(writer, item, options);
                    writer.WriteEndArray();
                    return;

                default:
                    JsonSerializer.Serialize(writer, value, value.GetType(), options);
                    return;
            }
        }
    }
}
