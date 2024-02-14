using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ActionCache.Common.Extensions.Internal;

namespace ActionCache.Common.Utilities;

public class ActionCacheRehydrationDescriptor
{
    public Dictionary<string, MethodInfo> MethodInfoCollection = new();
    public Dictionary<string, object> ControllerCollection = new();
}

public class ActionCacheDescriptorProvider
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IActionDescriptorCollectionProvider DescriptorCollectionProvider;

    public ActionCacheDescriptorProvider(
        IServiceProvider serviceProvider,
        IActionDescriptorCollectionProvider descriptorCollectionProvider
    )
    {
        ServiceProvider = serviceProvider;
        DescriptorCollectionProvider = descriptorCollectionProvider;
    }

    public ActionCacheRehydrationDescriptor GetControllerActionMethodInfo(string @namespace)
    {
        var descriptorCollection = new ActionCacheRehydrationDescriptor();
        if (DescriptorCollectionProvider.ActionDescriptors
                .TryGetControllerActionDescriptors(@namespace, out var controllerActionDescriptors))
        {
            var methodInfoKeyBuilder = new StringBuilder();
            foreach (var controllerActionDescriptor in controllerActionDescriptors)
            {
                var controller = ServiceProvider.GetRequiredService(controllerActionDescriptor.ControllerTypeInfo);
                var methodInfo = controller.GetType().GetMethod(controllerActionDescriptor.ActionName);
                if (methodInfo is not null)
                {
                    if (controllerActionDescriptor.RouteValues.TryGetValue("area", out var area))
                    {
                        methodInfoKeyBuilder.Append($"{area}:");
                    }

                    methodInfoKeyBuilder.AppendJoin(':', 
                        controllerActionDescriptor.ControllerName, 
                        controllerActionDescriptor.ActionName
                    );

                    var rehydrationDescriptorKey = methodInfoKeyBuilder.ToString();
                    descriptorCollection.MethodInfoCollection.Add(rehydrationDescriptorKey, methodInfo);
                    descriptorCollection.ControllerCollection.Add(rehydrationDescriptorKey, controller);
                    methodInfoKeyBuilder.Clear();
                }
            }
        }

        return descriptorCollection;
    }
}