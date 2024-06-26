using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Utilities;

internal class ActionCacheKeyBuilder
{
    protected readonly char KeySeparator = ':';
    protected readonly ActionDescriptor ActionDescriptor;

    public ActionCacheKeyBuilder(ActionDescriptor actionDescriptor) =>
        ActionDescriptor = actionDescriptor;

    protected string? RouteDataKey { get; set; }
    protected string? ActionArgKey { get; set; }

    public ActionCacheKeyBuilder WithRouteData(RouteData routeData)
    {
        RouteDataKey = ConcatKeyComponents(routeData.Values.Select(route => route.Value));
        return this;
    }

    public ActionCacheKeyBuilder WithActionArguments(IDictionary<string, object> actionArguments)
    {
        if (ActionDescriptor.TryGetKeyAttributes(out var attributes) && actionArguments.Any())
        {
            ActionArgKey = ConcatKeyComponents(attributes.GetArguments(actionArguments));
        }

        return this;
    }

    public string? Build()
    {
        if (string.IsNullOrWhiteSpace(RouteDataKey))
        {
            return default;
        }
        else
        {
            return string.IsNullOrWhiteSpace(ActionArgKey) ?
                RouteDataKey : ConcatKeyComponents([RouteDataKey, ActionArgKey]);
        }
    }

    private string ConcatKeyComponents(IEnumerable<object> components) => 
        string.Join(KeySeparator, components);
}