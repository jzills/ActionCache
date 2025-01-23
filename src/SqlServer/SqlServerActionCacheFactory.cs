using ActionCache.Common;
using ActionCache.Common.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ActionCache.SqlServer;

/// <summary>
/// Represents a factory for creating SqlServer action caches.
/// </summary>
public class SqlServerActionCacheFactory : ActionCacheFactoryBase
{
    /// <summary>
    /// A SqlServer cache implementation.
    /// </summary>
    protected readonly IDistributedCache SqlServerCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerActionCacheFactory"/> class.
    /// </summary>
    /// <param name="sqlServerCache">The SqlServer cache to use.</param>
    /// <param name="entryOptions">The global entry options used for creation when expiration times are not supplied.</param>
    /// <param name="refreshProvider">The refresh provider responsible for invoking cached controller actions.</param> 
    public SqlServerActionCacheFactory(
        IDistributedCache sqlServerCache,
        IOptions<ActionCacheEntryOptions> entryOptions,
        IActionCacheRefreshProvider refreshProvider
    ) : base(entryOptions.Value, refreshProvider)
    {
        SqlServerCache = sqlServerCache;
    }

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace) =>
        new SqlServerActionCache(@namespace, SqlServerCache, EntryOptions, RefreshProvider);

    /// <inheritdoc/>
    public override IActionCache? Create(string @namespace, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
    {
        var entryOptions = new ActionCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            SlidingExpiration = slidingExpiration
        };
        
        return new SqlServerActionCache(@namespace, SqlServerCache, entryOptions, RefreshProvider);
    }
}