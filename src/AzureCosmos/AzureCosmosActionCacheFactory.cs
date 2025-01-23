using ActionCache.Common;
using ActionCache.Common.Caching;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ActionCache.AzureCosmos;

/// <summary>
/// Represents a factory for creating Azure Cosmos Db action caches.
/// </summary>
public class AzureCosmosActionCacheFactory : ActionCacheFactoryBase
{
    /// <summary>
    /// An Azure Cosmos Db client implementation.
    /// </summary>
    protected readonly Container Cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCosmosActionCacheFactory"/> class.
    /// </summary>
    /// <param name="cache">The Azure Cosmos Db container to use.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param>
    /// <param name="refreshProvider">The refresh provider responsible for invoking cached controller actions.</param> 
    public AzureCosmosActionCacheFactory(
        Container cache,
        IOptions<ActionCacheEntryOptions> entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(entryOptions.Value, refreshProvider)
    {
        Cache = cache;
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