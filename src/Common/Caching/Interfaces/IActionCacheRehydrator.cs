namespace ActionCache.Common;

public interface IActionCacheRehydrator
{
    internal Task<IReadOnlyCollection<RehydrationResult>> GetRehydrationResultsAsync(string @namespace);
}