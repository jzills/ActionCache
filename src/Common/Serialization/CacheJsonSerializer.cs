using Newtonsoft.Json;

namespace ActionCache.Common.Serialization;

internal static class CacheJsonSerializer
{
    internal static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All,
        Converters = new List<JsonConverter> { new ActionArgumentsConverter() }
    };

    internal static string Serialize<T>(T? obj) => 
        JsonConvert.SerializeObject(obj, SerializerSettings);
    
    internal static T? Deserialize<T>(string json) => 
        JsonConvert.DeserializeObject<T>(json, SerializerSettings);
}