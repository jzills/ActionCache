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
using ActionCache.Common.Utilities;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheFilter_OkObjectResult_CacheEntry_Added
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

        var routeData = new RouteData(routeValues);
        var actionDescriptor = new ActionDescriptor();
        
        var metadata = new List<IFilterMetadata> { new ActionCacheAttribute { Namespace = @namespace } };
        var actionContext = new ActionContext(
            httpContext: new DefaultHttpContext(),
            routeData: routeData,
            actionDescriptor: actionDescriptor
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

        var key = new ActionCacheKeyBuilder()
            .WithRouteValues(routeData.Values)
            .WithActionArguments(null)
            .Build();

        var cacheResult = await cache.GetAsync<string>(key);
        Assert.That(cacheResult!.Equals("Foo"));
    }
}