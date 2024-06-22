using Microsoft.Extensions.DependencyInjection;
using ActionCache.Memory.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ActionCache.Attributes;
using ActionCache.Filters;
using ActionCache;

namespace Unit.Memory;

[TestFixture]
public class Test_ActionCacheFilter_OkObjectResult_CacheEntry_Added
{
    [Test]
    public async Task Test()
    {
        var @namespace = "Test";

        var serviceProvider = new ServiceCollection()
            .AddMemoryCache()
            .AddActionCacheMemory(options => options.SizeLimit = int.MaxValue)
            .BuildServiceProvider();

        var routeValues = new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" },
            { "action", "someAction" }
        };
        
        var metadata = new List<IFilterMetadata> { new ActionCacheAttribute { Namespace = @namespace } };
        var actionContext = new ActionContext(
            httpContext: new DefaultHttpContext(),
            routeData: new RouteData(routeValues),
            actionDescriptor: new ActionDescriptor()
        );

        var actionExecutingContext = new ActionExecutingContext(
            actionContext,
            metadata,
            new Dictionary<string, object>(),
            null);

        ActionExecutionDelegate next = () => {
            var context = new ActionExecutedContext(
                actionExecutingContext,
                metadata,
                null
            )
            {
                Result = new OkObjectResult("Foo")
            };

            return Task.FromResult(context);
        };

        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        var cache = cacheFactory.Create(@namespace)!;
        var filter = new ActionCacheFilter(cache);

        await filter.OnActionExecutionAsync(actionExecutingContext, next);
  
        var cacheResult = await cache.GetAsync<string>("someArea:someController:someAction");
        Assert.That(cacheResult!.Equals("Foo"));
    }
}