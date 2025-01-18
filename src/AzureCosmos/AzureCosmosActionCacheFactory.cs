using ActionCache.Common;
using ActionCache.Common.Caching;
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
    protected readonly Container Cache;

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
        Cache = cosmosClient.GetContainer(DatabaseId, Namespace.Assembly);
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace) =>
        new AzureCosmosActionCache(@namespace, Cache, EntryOptions, RefreshProvider);

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var entryOptions = new ActionCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            SlidingExpiration = slidingExpiration
        };

        return new AzureCosmosActionCache(@namespace, Cache, entryOptions, RefreshProvider);
    }
}