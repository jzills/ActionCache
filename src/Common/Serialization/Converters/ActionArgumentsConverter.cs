using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ActionCache.Common.Serialization;

internal class ActionArgumentsConverter : JsonConverter<Dictionary<string, object>>
{
    public override Dictionary<string, object> ReadJson(
        JsonReader reader, 
        Type objectType, 
        Dictionary<string, object>? existingValue, 
        bool hasExistingValue, 
        JsonSerializer serializer
    )
    {
        var result = new Dictionary<string, object>();

        var obj = JObject.Load(reader);
        foreach (var property in obj.Properties())
        {
            var value = ConvertToken(property.Value, serializer);
            result.Add(property.Name, value);
        }

        return result;
    }

    private object? ConvertToken(JToken token, JsonSerializer serializer) =>
        token.Type switch
        {
            JTokenType.String   => ConvertToStringOrGuid(token),
            JTokenType.Integer  => token.ToObject<long>(),
            JTokenType.Float    => token.ToObject<decimal>(),
            JTokenType.Boolean  => token.ToObject<bool>(),
            JTokenType.Null     => null,
            JTokenType.Object   => token.ToObject<object>(serializer),
            JTokenType.Array    => token.ToObject<object[]>(serializer),
            _                   => token.ToObject<object>(serializer)
        };

    private object ConvertToStringOrGuid(JToken token)
    {
        var value = token.ToString();

        if (Guid.TryParse(value, out var guidValue))
        {
            return guidValue;
        }
        else if (DateTime.TryParse(value, out var dateTimeValue))
        {
            return dateTimeValue;
        }
        else
        {
            return value;
        }
    }

    public override void WriteJson(
        JsonWriter writer, 
        Dictionary<string, object>? dictionary, 
        JsonSerializer serializer
    )
    {
        writer.WriteStartObject();

        if (dictionary is not null)
        {
            foreach (var (key, value) in dictionary)
            {
                writer.WritePropertyName(key);
                serializer.Serialize(writer, value); 
            }
        }

        writer.WriteEndObject();
    }
}