using ActionCache.Common.Utilities;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheKeyBuilder_WithRouteData_Hash
{
    [Test]
    public async Task Test()
    {
        var actionDescriptor = new ActionDescriptor();
        var routeData = new RouteData(new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" }, 
            { "action", "someAction" }
        });

        var key = new ActionCacheKeyBuilder(actionDescriptor)
            .WithRouteData(routeData)
            .Build();

        Assert.That(key, Is.EqualTo(KeyHashGenerator.ToHash("someArea:someController:someAction")));
    }
}