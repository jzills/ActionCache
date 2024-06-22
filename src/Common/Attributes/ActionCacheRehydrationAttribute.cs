using ActionCache.Filters;

namespace ActionCache.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal class ActionCacheRehydrationAttribute : ActionCacheRehydrationFilterFactory
{
}