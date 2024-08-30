namespace ActionCache.Memory;

/// <summary>
/// Interface for managing expiration token sources
/// </summary>
public interface IExpirationTokenSources
{
    /// <summary>
    /// Attempts to get or add a cancellation token source for the specified key
    /// </summary>
    /// <param name="key">The key associated with the cancellation token source</param>
    /// <param name="cancellationTokenSource">The cancellation token source associated with the key</param>
    /// <returns>True if the cancellation token source was retrieved or added successfully, false otherwise</returns>
    bool TryGetOrAdd(string key, out CancellationTokenSource cancellationTokenSource);
}