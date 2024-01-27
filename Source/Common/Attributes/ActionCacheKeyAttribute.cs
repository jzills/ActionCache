namespace ActionCache.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class ActionCacheKeyAttribute : Attribute
{
    public int Order { get; init; }
}