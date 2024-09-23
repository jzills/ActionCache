using ActionCache.Common.Utilities;
using Unit.TestUtiltiies.Data;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheKeyBuilder_WithActionArguments_Hash
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
        
        var key = new ActionCacheKeyBuilder(actionDescriptor, useHashForKeyComponents: true)
            .WithRouteData(new RouteData(routeValues))
            .WithActionArguments(actionArguments)
            .Build();

        var routeValuesHash = KeyHashGenerator.ToHash(string.Join(":", routeValues.Values));
        var actionArgumentsHash = KeyHashGenerator.ToHash(string.Join(":", actionArguments.Values));
        Assert.That(key, Is.EqualTo(KeyHashGenerator.ToHash(string.Join(":", [routeValuesHash, actionArgumentsHash]))));
    }
}