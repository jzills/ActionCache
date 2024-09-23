using ActionCache.Common.Utilities;
using Unit.TestUtiltiies.Data;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheKeyBuilder_WithActionArguments_PlainText
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetControllerDescriptors))]
    public void Test(
        IEnumerable<ControllerParameterDescriptor> controllerParameterDescriptors, 
        RouteValueDictionary routeValues, 
        Dictionary<string, object> actionArguments
    )
    {
        var actionDescriptor = new ActionDescriptor
        {
            Parameters = controllerParameterDescriptors.ToArray()
        };
        
        var key = new ActionCacheKeyBuilder(actionDescriptor, useHashForKeyComponents: false)
            .WithRouteData(new RouteData(routeValues))
            .WithActionArguments(actionArguments)
            .Build();

        Assert.That(key, Is.EqualTo(string.Join(":", routeValues.Values.Concat(actionArguments.Values))));
    }
}