namespace ActionCache.Memory;

public interface IExpirationTokenSources
{
    bool TryGetOrAdd(string key, out CancellationTokenSource cancellationTokenSource);
}