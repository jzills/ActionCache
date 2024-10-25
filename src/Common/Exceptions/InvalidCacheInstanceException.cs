namespace ActionCache.Exceptions;

public class InvalidCacheInstanceException : Exception
{
    public InvalidCacheInstanceException()
        : base("One or more cache instances are null.")
    {
    }
}