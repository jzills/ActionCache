using System.Net;
using ActionCache.AzureCosmos.Exceptions;
using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ActionCache.AzureCosmos;

/// <summary>
/// Provides functionality for creating and managing Azure Cosmos DB action caches.
/// </summary>
public class AzureCosmosActionCacheProvider
{
    /// <summary>
    /// The Cosmos DB client used to interact with the Azure Cosmos DB service.
    /// </summary>
    protected readonly CosmosClient CosmosClient;

    /// <summary>
    /// Options for configuring action cache entries.
    /// </summary>
    protected readonly IOptions<ActionCacheEntryOptions> EntryOptions;

    /// <summary>
    /// The provider responsible for refreshing action caches.
    /// </summary>
    protected readonly IActionCacheRefreshProvider RefreshProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCosmosActionCacheProvider"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Cosmos DB client used for interacting with Azure Cosmos DB.</param>
    /// <param name="entryOptions">The options for configuring action cache entries.</param>
    /// <param name="refreshProvider">The provider responsible for refreshing action caches.</param>
    public AzureCosmosActionCacheProvider(
        CosmosClient cosmosClient,
        IOptions<ActionCacheEntryOptions> entryOptions,
        IActionCacheRefreshProvider refreshProvider
    )
    {
        CosmosClient = cosmosClient;
        EntryOptions = entryOptions;
        RefreshProvider = refreshProvider;
    }

    /// <summary>
    /// Creates an instance of <see cref="AzureCosmosActionCacheFactory"/> for the specified database.
    /// </summary>
    /// <param name="databaseId">The identifier of the Azure Cosmos DB database.</param>
    /// <returns>A task that represents the asynchronous operation. 
    /// The task result contains the created <see cref="AzureCosmosActionCacheFactory"/>.</returns>
    /// <exception cref="AzureCosmosDatabaseNotFoundOrCreated">
    /// Thrown when the database could not be found or created.
    /// </exception>
    /// <exception cref="AzureCosmosContainerNotFoundOrCreated">
    /// Thrown when the container could not be found or created.
    /// </exception>
    public async Task<AzureCosmosActionCacheFactory> CreateAsync(string databaseId)
    {
        var databaseResponse = await CosmosClient
            .CreateDatabaseIfNotExistsAsync(databaseId);

        if (IsSuccessStatusCode(databaseResponse.StatusCode))
        {
            var containerResponse = await databaseResponse.Database
                .CreateContainerIfNotExistsAsync(new ContainerProperties 
                { 
                    Id = Namespace.Assembly,
                    PartitionKeyPath = "/namespace",
                    DefaultTimeToLive = -1
                });

            if (IsSuccessStatusCode(containerResponse.StatusCode))
            {
                return new AzureCosmosActionCacheFactory(
                    containerResponse.Container,
                    EntryOptions,
                    RefreshProvider
                );
            }
            else
            {
                throw new AzureCosmosContainerNotFoundOrCreated(containerResponse);
            }
        }
        else
        {
            throw new AzureCosmosDatabaseNotFoundOrCreated(databaseResponse);
        }
    }

    /// <summary>
    /// Determines whether the provided HTTP status code indicates a successful response.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to evaluate.</param>
    /// <returns>
    /// <see langword="true"/> if the status code indicates success; otherwise, <see langword="false"/>.
    /// </returns>
    private bool IsSuccessStatusCode(HttpStatusCode statusCode) =>
        statusCode == HttpStatusCode.OK || 
        statusCode == HttpStatusCode.Created;
}