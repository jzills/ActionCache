using System.Text.Json;
using ActionCache.Common.Keys;
using ActionCache.Common.Utilities;
using ActionCache.Utilities;
using Microsoft.AspNetCore.Routing;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheKeyComponentsBuilder
{
    [Test]
    public void Test()
    {
        var actionArguments = new Dictionary<string, object>
        {
            { "Foo", "Bar" },
            { "Biz", 22222 }
        };

        var routeValues = new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" },
            { "action", "someAction" }
        };

        var key = new ActionCacheKeyBuilder()
            .WithActionArguments(actionArguments)
            .WithRouteValues(routeValues)
            .Build();

        var keyComponents = new ActionCacheKeyComponentsBuilder(key).Build();

        Assert.That(keyComponents.ActionArguments.ContainsKey("Foo"));
        Assert.That((string)keyComponents.ActionArguments["Foo"], Is.EqualTo("Bar"));

        Assert.That(keyComponents.ActionArguments.ContainsKey("Biz"));
        Assert.That((long?)keyComponents.ActionArguments["Biz"], Is.EqualTo(22222));

        Assert.That(keyComponents.RouteValues.ContainsKey("area"));
        Assert.That((string?)keyComponents.RouteValues["area"], Is.EqualTo("someArea"));

        Assert.That(keyComponents.RouteValues.ContainsKey("controller"));
        Assert.That((string?)keyComponents.RouteValues["controller"], Is.EqualTo("someController"));

        Assert.That(keyComponents.RouteValues.ContainsKey("action"));
        Assert.That((string?)keyComponents.RouteValues["action"], Is.EqualTo("someAction"));
    }
}