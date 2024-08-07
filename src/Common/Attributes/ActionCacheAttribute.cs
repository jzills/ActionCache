using ActionCache.Filters;

namespace ActionCache.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ActionCacheAttribute : ActionCacheFilterFactory
{
}