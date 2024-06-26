using System.Reflection;

namespace ActionCache.Common.Utilities;

public class ActionCacheRehydrationDescriptor
{
    public Dictionary<string, MethodInfo> MethodInfos = new();
    public Dictionary<string, object> Controllers = new();

    public void Add(string key, MethodInfo methodInfo, object controller)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        MethodInfos.Add(key, methodInfo);
        Controllers.Add(key, controller);
    }
}