using ActionCache.Filters;

namespace ActionCache.Attributes;

/// <summary>
/// Provides an attribute to specify rehydration behavior for cached action results.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal class ActionCacheRehydrationAttribute : ActionCacheRehydrationFilterFactory
{
}