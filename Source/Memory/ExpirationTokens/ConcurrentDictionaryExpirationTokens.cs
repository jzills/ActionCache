using System.Collections.Concurrent;

namespace ActionCache.Memory;

public class ConcurrentDictionaryExpirationTokens
{
    protected ConcurrentDictionary<string, CancellationTokenSource> Tokens = new();
    public bool TryGetOrAdd(string key, out CancellationTokenSource cancellationTokenSource)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            cancellationTokenSource = Tokens.GetOrAdd(key, new CancellationTokenSource());

            // If the token has been cancelled, then remove it 
            // and add a new cancellationTokenSource for future cache entries
            if (cancellationTokenSource.IsCancellationRequested)
            {
                if (Tokens.TryRemove(key, out _))
                {
                    cancellationTokenSource = Tokens.GetOrAdd(key, new CancellationTokenSource());
                }
            }

            return true;
        }
        else
        {
            cancellationTokenSource = default!;
            return false; 
        }
    }
}