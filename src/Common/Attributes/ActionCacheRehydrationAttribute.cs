using ActionCache.Filters;

namespace ActionCache.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ActionCacheRehydrationAttribute : ActionCacheRehydrationFilterFactory
{
}