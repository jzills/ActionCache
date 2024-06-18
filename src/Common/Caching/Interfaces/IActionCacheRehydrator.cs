namespace ActionCache.Common;

public interface IActionCacheRehydrator
{
    Task<IReadOnlyCollection<RehydrationResult>> GetRehydrationResultsAsync(string @namespace);
}