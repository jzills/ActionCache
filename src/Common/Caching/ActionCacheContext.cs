using ActionCache.Common.Concurrency;
using ActionCache.Utilities;

namespace ActionCache.Common.Caching;

public class ActionCacheContext<TLock> where TLock : CacheLock
{
    public Namespace Namespace { get; set; }
    public ActionCacheEntryOptions EntryOptions { get; set; }
    public IActionCacheRefreshProvider RefreshProvider { get; set; }
    public ICacheLocker<TLock> CacheLocker { get; set; }
}