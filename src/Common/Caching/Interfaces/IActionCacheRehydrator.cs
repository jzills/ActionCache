namespace ActionCache.Common;

/// <summary>
/// Defines an interface for rehydrating action cache results.
/// </summary>
public interface IActionCacheRehydrator
{
    /// <summary>
    /// Asynchronously retrieves a collection of rehydration results.
    /// </summary>
    /// <param name="namespace">The namespace for which rehydration results are retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the read-only collection of rehydration results.</returns>
    internal Task<IReadOnlyCollection<RehydrationResult>> GetRehydrationResultsAsync(string @namespace);
}