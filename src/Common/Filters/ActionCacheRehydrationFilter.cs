using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ActionCache.Common.Extensions.Internal;

namespace ActionCache.Filters;

public class ActionCacheRehydrationFilter : IAsyncResultFilter
{
    private readonly IActionCache _cache;
    private readonly IActionDescriptorCollectionProvider _descriptorProvider;
    private readonly IServiceProvider _serviceProvider;
    public ActionCacheRehydrationFilter(
        IActionCache cache,
        IActionDescriptorCollectionProvider descriptorProvider,
        IServiceProvider serviceProvider
    )
    {
        _cache = cache;
        _descriptorProvider = descriptorProvider;
        _serviceProvider = serviceProvider;
    }
    
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (_descriptorProvider.ActionDescriptors.TryGetControllerActionDescriptors("Namespace1", out var descriptors))
        {
            foreach (var descriptor in descriptors)
            {
                var controller = _serviceProvider.GetRequiredService(descriptor.ControllerTypeInfo);
                var methodInfo = controller.GetType().GetMethod(descriptor.ActionName);
                if (methodInfo is not null)
                {
                    // TODO: In order to get the parameters for each
                    // rehydration action, the route values will need to be stored
                    // in the cache as well
                    var result = methodInfo.Invoke(controller, [ 99, DateTime.Now ]);
                    if (result is OkObjectResult okObjectResult)
                    {
                        // Get keys from namespace set in Redis
                        // TODO: Implement same structure as above for MemoryCache..
                        //       since MemoryCache implementation uses cancellationToken, however
                        //       to support rehydration, we need to know all keys in the namespace set
                        //await _cache.SetAsync("", okObjectResult.Value);
                    }
                }
            }
        }

        await next();
    }
}
