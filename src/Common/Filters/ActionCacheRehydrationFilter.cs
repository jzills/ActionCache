using System.Reflection;
using ActionCache.Attributes;
using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Filters;

public class ActionCacheRehydrationFilter : IAsyncActionFilter
{
    private readonly IActionCache _cache;
    private readonly IActionDescriptorCollectionProvider _descriptorProvider;
    public ActionCacheRehydrationFilter(
        IActionCache cache,
        IActionDescriptorCollectionProvider descriptorProvider
    ) => (_cache, _descriptorProvider) = (cache, descriptorProvider);
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();

        var controllerActionDescriptors = _descriptorProvider.ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .Where(descriptor => descriptor.MethodInfo
                .GetCustomAttribute<ActionCacheAttribute>()?.Namespace.Contains("Namespace1") ?? false);

        foreach (var descriptor in controllerActionDescriptors)
        {
            var controller = context.HttpContext.RequestServices.GetRequiredService(descriptor.ControllerTypeInfo);
            var methodInfo = controller.GetType().GetMethod(descriptor.ActionName);
            var result = methodInfo.Invoke(controller, [ 99, DateTime.Now ]);
            if (result is OkObjectResult okObjectResult)
            {
                var value = okObjectResult.Value;
            }
        }
    }  
}
