using System.Reflection;

namespace ActionCache.Common.Utilities;

public class ActionCacheRehydrationDescriptor
{
    public Dictionary<string, MethodInfo> MethodInfos = new();
    public Dictionary<string, object> Controllers = new();
}