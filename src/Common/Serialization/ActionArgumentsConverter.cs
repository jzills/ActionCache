using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActionCache.Common.Serialization;

public class ActionArgumentsConverter : JsonConverter<Dictionary<string, object>>
{
    public override Dictionary<string, object> ReadJson(JsonReader reader, Type objectType, Dictionary<string, object> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var result = new Dictionary<string, object>();

        JObject obj = JObject.Load(reader);
        foreach (var property in obj.Properties())
        {
            string key = property.Name;
            JToken valueToken = property.Value;

            object value = ConvertToken(valueToken, serializer);
            result.Add(key, value);
        }

        return result;
    }

    private object ConvertToken(JToken token, JsonSerializer serializer)
    {
        switch (token.Type)
        {
            case JTokenType.String:
                var stringValue = token.ToString();
                if (Guid.TryParse(stringValue, out var guidValue))
                {
                    return guidValue;
                }
                else if (DateTime.TryParse(stringValue, out var dateTimeValue))
                {
                    return dateTimeValue;
                }
                return stringValue;
            case JTokenType.Integer:
                return token.ToObject<long>();
            case JTokenType.Float:
                return token.ToObject<decimal>();
            case JTokenType.Boolean:
                return token.ToObject<bool>();
            case JTokenType.Null:
                return null;
            case JTokenType.Object:
                // For complex objects, deserialize them recursively
                return token.ToObject<object>(serializer);
            case JTokenType.Array:
                // Handle arrays of values, which may contain complex types
                return token.ToObject<object[]>(serializer);
            default:
                // Fallback for any other types (still using token.ToObject with the serializer)
                return token.ToObject<object>(serializer);
        }
    }

    public override void WriteJson(JsonWriter writer, Dictionary<string, object> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key);
            serializer.Serialize(writer, kvp.Value); // Use the serializer to handle complex types
        }
        writer.WriteEndObject();
    }
}