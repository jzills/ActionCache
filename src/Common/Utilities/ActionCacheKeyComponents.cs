using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Utilities;

public class ActionCacheKeyComponents
{
    public RouteValueDictionary? RouteValues { get; set; }
    public IReadOnlyDictionary<string, object>? ActionArguments { get; set; }
    public string Serialize()
    {
        var route = $"route={JsonSerializer.Serialize(RouteValues)}";
        var args  = $"args={JsonSerializer.Serialize(ActionArguments)}";
        return $"{route}&{args}";
    }
}