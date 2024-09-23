using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ActionCache.Attributes;
using ActionCache.Filters;
using ActionCache;
using Unit.TestUtiltiies.Data;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheFilter_Eviction_CacheEntry_Removed
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetServiceProviders))]
    public async Task Test(IServiceProvider serviceProvider)
    {
        var @namespace = "Test";
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

        await cache.SetAsync("someArea:someController:someAction", "Foo");

        var filter = new ActionCacheEvictionFilter(cache);

        await filter.OnActionExecutionAsync(actionExecutingContext, next);
  
        var cacheResult = await cache.GetAsync<string>("someArea:someController:someAction");
        Assert.That(cacheResult, Is.Null);
    }
}