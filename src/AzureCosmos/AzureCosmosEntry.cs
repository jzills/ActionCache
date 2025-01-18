using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ActionCache.AzureCosmos;

/// <summary>
/// Represents an entry in Azure Cosmos.
/// </summary>
public class AzureCosmosEntry
{
    /// <summary>
    /// Gets or sets the unique identifier for the entry.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonProperty(PropertyName = "id")] 
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the namespace associated with the entry.
    /// </summary>
    [JsonPropertyName("namespace")]
    [JsonProperty(PropertyName = "namespace")] 
    public required string Namespace { get; set; }

    /// <summary>
    /// Gets or sets the value associated with the entry.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonProperty(PropertyName = "value")]  
    public required string Value { get; set; }
}