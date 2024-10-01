using System.Text.Json;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Keys;

public class ActionCacheKeyComponents
{
    public RouteValueDictionary? RouteValues { get; set; } = new();
    public IReadOnlyDictionary<string, object>? ActionArguments { get; set; } = new Dictionary<string, object>();
    public string Serialize()
    {
        var route = $"route={JsonSerializer.Serialize(RouteValues)}";
        var args  = $"args={JsonSerializer.Serialize(ActionArguments)}";
        return $"{route}&{args}";
    }
}