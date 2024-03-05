using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ActionCache.Common.Extensions.Internal;

namespace ActionCache.Common.Utilities;

public class ActionCacheDescriptorProvider
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly ActionDescriptorCollection ActionDescriptors;
    protected readonly StringBuilder KeyBuilder;

    public ActionCacheDescriptorProvider(
        IServiceProvider serviceProvider,
        IActionDescriptorCollectionProvider descriptorProvider
    )
    {
        ServiceProvider = serviceProvider;
        ActionDescriptors = descriptorProvider.ActionDescriptors;
        KeyBuilder = new();
    }

    public ActionCacheRehydrationDescriptor GetControllerActionMethodInfo(string @namespace)
    {
        var descriptors = new ActionCacheRehydrationDescriptor();
        if (ActionDescriptors.TryGetControllerActionDescriptors(@namespace, out var controllerActionDescriptors))
        {
            foreach (var (areaName, controllerName, actionName, controllerTypeInfo) in controllerActionDescriptors)
            {
                var controller = ServiceProvider.GetRequiredService(controllerTypeInfo);
                var methodInfo = controller.GetType().GetMethod(actionName);
                if (methodInfo is not null)
                {
                    var key = CreateKey(areaName, controllerName, actionName);
                    descriptors.MethodInfos.Add(key, methodInfo);
                    descriptors.Controllers.Add(key, controller);
                }
            }
        }

        return descriptors;
    }

    private string CreateKey(string? areaName, string controllerName, string actionName)
    {
        var key = KeyBuilder.AppendJoinNonNull(':', areaName, controllerName, actionName).ToString();
        KeyBuilder.Clear();
        return key;
    }
}