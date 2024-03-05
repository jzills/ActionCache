using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ActionCache.Common.Utilities;

public static class ControllerActionDescriptorExtensions
{
    public static void Deconstruct(
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