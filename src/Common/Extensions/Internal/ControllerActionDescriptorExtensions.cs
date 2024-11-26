using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace ActionCache.Common.Extensions.Internal;

/// <summary>
/// Provides extension methods for <see cref="ControllerActionDescriptor"/>.
/// </summary>
internal static class ControllerActionDescriptorExtensions
{
    /// <summary>
    /// Deconstructs the <see cref="ControllerActionDescriptor"/> into easier to manage parts.
    /// </summary>
    /// <param name="descriptor">The ControllerActionDescriptor to deconstruct.</param>
    /// <param name="areaName">Output of the area name from the route values.</param>
    /// <param name="controllerName">Name of the controller.</param>
    /// <param name="actionName">Name of the action.</param>
    /// <param name="controllerTypeInfo">Type information of the controller.</param>
    internal static void Deconstruct(
        this ControllerActionDescriptor descriptor, 
        out string? areaName,
        out string controllerName, 
        out string actionName, 
        out TypeInfo controllerTypeInfo
    )
    {
        descriptor.RouteValues.TryGetValue("area", out areaName);
        
        actionName          = descriptor.ActionName;
        controllerName      = descriptor.ControllerName;
        controllerTypeInfo  = descriptor.ControllerTypeInfo;
    }
}