using ActionCache.Common.Extensions.Internal;
using ActionCache.Common.Serialization;
using ActionCache.Utilities;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Keys;

public class ActionCacheKeyComponents
{
    public const string RouteValuesKey = nameof(RouteValuesKey);
    public const string ActionArgumentsKey = nameof(ActionArgumentsKey);

    public RouteValueDictionary? RouteValues { get; set; } = new();
    public Dictionary<string, object>? ActionArguments { get; set; } = new Dictionary<string, object>();
    
    /// <summary>
    /// Serializes route values and action arguments as key value pairs.
    /// </summary>
    /// <returns>A serialized string representing the key components.</returns>
    public string Serialize()
    {
        var routeValues = CacheJsonSerializer.Serialize(RouteValues);
        var actionArguments = CacheJsonSerializer.Serialize(ActionArguments);
        return $"{RouteValuesKey}={routeValues}&{ActionArgumentsKey}={actionArguments}";
    }

    /// <summary>
    /// Deconstructs route values into string representations of an area, controller and action.
    /// </summary>
    /// <param name="area"></param>
    /// <param name="controller"></param>
    /// <param name="action"></param>
    public void Deconstruct(out string? area, out string? controller, out string? action)
    {
        ArgumentNullException.ThrowIfNull(RouteValues);

        RouteValues.TryGetStringValue(RouteKeys.Area, out area);
        RouteValues.TryGetStringValue(RouteKeys.Controller, out controller);
        RouteValues.TryGetStringValue(RouteKeys.Action, out action);
    }
}