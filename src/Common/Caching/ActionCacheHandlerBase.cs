namespace ActionCache.Common.Caching;

public class ActionCacheHandlerBase : IActionCacheHandler
{
    protected IActionCache? Next { get; set; } 

    public IActionCache SetNext(IActionCache next)
    {
        Next = next;
        return next;
    }
}