using ActionCache.Redis;

namespace ActionCache.Common;

public interface IActionCacheRehydrator
{
    Task SetAsync<TValue>(string key, TValue value);
    Task<IReadOnlyCollection<RehydrationResult>> GetRehydrationResultsAsync(string @namespace);
}