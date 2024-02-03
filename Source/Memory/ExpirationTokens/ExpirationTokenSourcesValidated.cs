namespace ActionCache.Memory;

public class ExpirationTokenSourcesValidated : IExpirationTokenSources
{
    protected readonly IExpirationTokenSources Next;
    public ExpirationTokenSourcesValidated(IExpirationTokenSources next) => Next = next;

    public bool TryGetOrAdd(string key, out CancellationTokenSource cancellationTokenSource)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            return Next.TryGetOrAdd(key, out cancellationTokenSource);
        }
        else
        {
            cancellationTokenSource = default!;
            return false;
        }
    }
}