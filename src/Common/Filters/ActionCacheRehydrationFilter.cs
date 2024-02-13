using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ActionCache.Common.Utilities;

namespace ActionCache.Filters;

public class ActionCacheRehydrationFilter : IAsyncResultFilter
{
    protected readonly IActionCache Cache;
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;

    public ActionCacheRehydrationFilter(
        IActionCache cache,
        ActionCacheDescriptorProvider descriptorProvider
    )
    {
        Cache = cache;
        DescriptorProvider = descriptorProvider;
    }
    
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var descriptorCollection = DescriptorProvider.GetControllerActionMethodInfo("Namespace1");
        if (descriptorCollection.MethodInfoCollection.Any())
        {
            foreach (var (route, methodInfo) in descriptorCollection.MethodInfoCollection)
            {
                var controller = descriptorCollection.ControllerCollection[route];
                var result = methodInfo.Invoke(controller, [ 99, DateTime.Now ]);
                if (result is OkObjectResult okObjectResult)
                {
                    // Get keys from namespace set in Redis
                    // TODO: Implement same structure as above for MemoryCache..
                    //       since MemoryCache implementation uses cancellationToken, however
                    //       to support rehydration, we need to know all keys in the namespace set
                    await Cache.SetAsync("", okObjectResult.Value);
                }
            }
        }

        await next();
    }
}
