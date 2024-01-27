namespace ActionCache;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ActionCacheAttribute : ActionCacheFilterFactory
{
}