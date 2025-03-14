using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ActionCache.Attributes;
using ActionCache.Filters;
using ActionCache;
using ActionCache.Common.Keys;
using Unit.TestUtiltiies.Data;
using Microsoft.AspNetCore.Routing.Template;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheFilter_OkObjectResult_CacheEntry_Added
{
    IActionCache Cache;

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

        var binderFactory = serviceProvider.GetRequiredService<TemplateBinderFactory>();
        var cacheFactory = serviceProvider.GetRequiredService<IActionCacheFactory>();
        Cache = cacheFactory.Create(@namespace)!;
        var filter = new ActionCacheFilter(Cache, binderFactory);

        await filter.OnActionExecutionAsync(actionExecutingContext, next);

        var key = new ActionCacheKeyBuilder()
            .WithRouteValues(routeData.Values)
            .Build();

        var cacheResult = await Cache.GetAsync<IActionResult>(key);
       
        Assert.IsInstanceOf<OkObjectResult>(cacheResult);
        Assert.That((cacheResult as OkObjectResult)?.Value, Is.EqualTo("Foo"));
    }

    [TearDown]
    public async Task TearDown()
    {
        await Cache.RemoveAsync();
    }
}