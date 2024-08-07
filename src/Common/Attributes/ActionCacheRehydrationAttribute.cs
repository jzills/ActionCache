using ActionCache.Filters;

namespace ActionCache.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class ActionCacheRehydrationAttribute : ActionCacheRehydrationFilterFactory
{
}