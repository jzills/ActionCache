using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Utilities;

/// <summary>
/// Provides functionality to build cache keys for action methods.
/// </summary>
public class ActionCacheKeyBuilder_Backup
{
    /// <summary>
    /// The key separator used to delineate between key components.
    /// </summary>
    protected readonly char KeySeparator = ':';

    /// <summary>
    /// The action descriptor related to the incoming request. 
    /// </summary>
    protected readonly ActionDescriptor ActionDescriptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheKeyBuilder"/> class.
    /// </summary>
    /// <param name="actionDescriptor">The action descriptor associated with the cache key.</param>
    /// <param name="useHashForKeyComponents">A flag indicating if the builder should hash the cache key.</param>
    public ActionCacheKeyBuilder_Backup(
        ActionDescriptor actionDescriptor, 
        bool useHashForKeyComponents = true
    )
    {
        ActionDescriptor = actionDescriptor;
        UseHashForKeyComponents = useHashForKeyComponents;
    }

    /// <summary>
    /// A key component derived from the route data associated with an incoming request. 
    /// </summary>
    protected string? RouteDataKey { get; set; }

    /// <summary>
    /// A key component derived from the action arguments associated with an incoming request. 
    /// </summary>
    protected string? ActionArgKey { get; set; }

    /// <summary>
    /// A flag indicating if the builder should hash the cache key.
    /// </summary>
    protected bool UseHashForKeyComponents { get; set; } = true;

    /// <summary>
    /// Includes route data in the cache key.
    /// </summary>
    /// <param name="routeData">Route data for the action.</param>
    /// <returns>Returns itself for chaining.</returns>
    public ActionCacheKeyBuilder_Backup WithRouteData(RouteData routeData)
    {
        RouteDataKey = ConcatKeyComponents(routeData.Values.Select(route => route.Value));
        return this;
    }

    /// <summary>
    /// Includes action arguments in the cache key.
    /// </summary>
    /// <param name="actionArguments">Arguments for the action.</param>
    /// <returns>Returns itself for chaining.</returns>
    public ActionCacheKeyBuilder_Backup WithActionArguments(IDictionary<string, object> actionArguments)
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
                RouteDataKey : ConcatKeyComponents([RouteDataKey, ActionArgKey]);
        }
    }

    /// <summary>
    /// Concatenates key components using the configured separator.
    /// </summary>
    /// <param name="components">Components to concatenate.</param>
    /// <returns>The concatenated string.</returns>
    private string ConcatKeyComponents(IEnumerable<object> components)
    {
        var keyComponents = string.Join(KeySeparator, components);
        return UseHashForKeyComponents ?
            KeyHashGenerator.ToHash(keyComponents) : keyComponents;
    }
}