using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Memory;
using ActionCache.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ActionCache.AzureCosmos;

/// <summary>
/// Represents a factory for creating Azure Cosmos Db action caches.
/// </summary>
public class AzureCosmosActionCacheFactory : ActionCacheFactoryBase
{
    /// TODO: Move to IServiceCollection configuration
    public readonly string DatabaseId = "SampleDb";

    /// <summary>
    /// An Azure Cosmos Db client implementation.
    /// </summary>
    protected readonly CosmosClient CosmosClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCosmosActionCacheFactory"/> class.
    /// </summary>
    /// <param name="cosmosClient">The Azure Cosmos Db client to use.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param>
    /// <param name="refreshProvider">The refresh provider responsible for invoking cached controller actions.</param> 
    public AzureCosmosActionCacheFactory(
        CosmosClient cosmosClient,
        IOptions<ActionCacheEntryOptions> entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(CacheType.AzureCosmos, entryOptions.Value, refreshProvider)
    {
        CosmosClient = cosmosClient;
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace)
    {
        var cache = CosmosClient.GetDatabase(DatabaseId).CreateContainerIfNotExistsAsync(new ContainerProperties
        {
            Id = Namespace.Assembly,
            PartitionKeyPath = "/namespace"
        }).Result;

        return new AzureCosmosActionCache(@namespace, cache, EntryOptions, RefreshProvider);
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var entryOptions = new ActionCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            SlidingExpiration = slidingExpiration
        };

        var cache = CosmosClient.GetContainer(DatabaseId, Namespace.Assembly);
        return new AzureCosmosActionCache(@namespace, cache, entryOptions, RefreshProvider);
    }
}