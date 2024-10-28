namespace ActionCache.Exceptions;

public class InvalidCacheInstanceException : Exception
{
    public InvalidCacheInstanceException(string? message = null)
        : base(message ?? "One or more cache instances are null.")
    {
    }
}