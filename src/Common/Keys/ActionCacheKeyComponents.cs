using ActionCache.Common.Serialization;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Keys;

public class ActionCacheKeyComponents
{
    public RouteValueDictionary? RouteValues { get; set; } = new();
    public Dictionary<string, object>? ActionArguments { get; set; } = new Dictionary<string, object>();
    public string Serialize()
    {
        var route = $"route={CacheJsonSerializer.Serialize(RouteValues)}";
        var args  = $"args={CacheJsonSerializer.Serialize(ActionArguments)}";
        return $"{route}&{args}";
    }
    public void Deconstruct(out string? area, out string? controller, out string? action)
    {
        ArgumentNullException.ThrowIfNull(RouteValues);

        area = null;
        if (RouteValues.ContainsKey("area"))
        {
            area = (string?)RouteValues["area"];
        }

        controller = null;
        if (RouteValues.ContainsKey("controller"))
        {
            controller = (string?)RouteValues["controller"];
        }

        action = null;
        if (RouteValues.ContainsKey("action"))
        {
            action = (string?)RouteValues["action"];
        }
    }
}