using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using ActionCache.Common.Extensions;

namespace ActionCache.Common.Utilities;

/// <summary>
/// Provides functionality to build cache keys for action methods.
/// </summary>
internal class ActionCacheKeyBuilder
{
    protected readonly char KeySeparator = ':';
    protected readonly ActionDescriptor ActionDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheKeyBuilder"/> class.
    /// </summary>
    /// <param name="actionDescriptor">The action descriptor associated with the cache key.</param>
    public ActionCacheKeyBuilder(ActionDescriptor actionDescriptor) =>
        ActionDescriptor = actionDescriptor;

    protected string? RouteDataKey { get; set; }
    protected string? ActionArgKey { get; set; }

    /// <summary>
    /// Includes route data in the cache key.
    /// </summary>
    /// <param name="routeData">Route data for the action.</param>
    /// <returns>Returns itself for chaining.</returns>
    public ActionCacheKeyBuilder WithRouteData(RouteData routeData)
    {
        RouteDataKey = ConcatKeyComponents(routeData.Values.Select(route => route.Value));
        return this;
    }

    /// <summary>
    /// Includes action arguments in the cache key.
    /// </summary>
    /// <param name="actionArguments">Arguments for the action.</param>
    /// <returns>Returns itself for chaining.</returns>
    public ActionCacheKeyBuilder WithActionArguments(IDictionary<string, object> actionArguments)
    {
        if (ActionDescriptor.TryGetKeyAttributes(out var attributes) && actionArguments.Any())
        {
            ActionArgKey = ConcatKeyComponents(attributes.GetArguments(actionArguments));
        }

        return this;
    }

    /// <summary>
    /// Builds the final cache key.
    /// </summary>
    /// <returns>The constructed cache key, or null if critical components are missing.</returns>
    public string? Build()
    {
        if (string.IsNullOrWhiteSpace(RouteDataKey))
        {
            return default;
        }
        else
        {
            return string.IsNullOrWhiteSpace(ActionArgKey) ?
                RouteDataKey : ConcatKeyComponents(new[] { RouteDataKey, ActionArgKey });
        }
    }

    /// <summary>
    /// Concatenates key components using the configured separator.
    /// </summary>
    /// <param name="components">Components to concatenate.</param>
    /// <returns>The concatenated string.</returns>
    private string ConcatKeyComponents(IEnumerable<object> components) => 
        string.Join(KeySeparator, components);
}