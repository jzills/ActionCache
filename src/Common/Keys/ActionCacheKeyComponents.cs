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
    public void Deconstruct(out string? area, out string? controller, out string? action)
    {
        ArgumentNullException.ThrowIfNull(RouteValues);

        var areaElement = (JsonElement?)RouteValues.GetValueOrDefault("area");
        area = areaElement?.GetString();

        var controllerElement = (JsonElement?)RouteValues.GetValueOrDefault("controller");
        controller = controllerElement?.GetString();

        var actionElement = (JsonElement?)RouteValues.GetValueOrDefault("action");
        action = actionElement?.GetString();
    }
}