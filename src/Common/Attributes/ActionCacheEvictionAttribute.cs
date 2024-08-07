using ActionCache.Filters;

namespace ActionCache.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ActionCacheEvictionAttribute : ActionCacheEvictionFilterFactory
{
}