using System.Reflection;
using ActionCache.Attributes;

namespace ActionCache.Common.Extensions.Internal;

public static class MethodInfoExtensions
{
    public static bool HasActionCacheAttribute(this MethodInfo source, string @namespace) =>
        source.GetCustomAttribute<ActionCacheAttribute>()?
            .Namespace.Contains(@namespace) ?? false;
}