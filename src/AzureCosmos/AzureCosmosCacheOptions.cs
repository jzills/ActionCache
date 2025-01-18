using Microsoft.Azure.Cosmos;

namespace ActionCache.AzureCosmos;

/// <summary>
/// Represents configuration options for the Azure Cosmos cache.
/// </summary>
public class AzureCosmosCacheOptions
{
    /// <summary>
    /// Gets or sets the connection string for the Azure Cosmos cache.
    /// </summary>
    /// <remarks>
    /// The <see cref="ConnectionString"/> is required to establish a connection to the Cosmos database.
    /// </remarks>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the options for configuring the Cosmos client.
    /// </summary>
    public CosmosClientOptions? CosmosClientOptions { get; set; }
}