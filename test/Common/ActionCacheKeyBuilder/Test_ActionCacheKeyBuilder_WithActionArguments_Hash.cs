using ActionCache.Common.Utilities;
using Unit.TestUtiltiies.Data;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using ActionCache.Common.Extensions;
using System.Text.Json;

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

        var actionArgumentValues = new List<object>();
        foreach (var value in actionArguments.Values)
        {
            if (value.GetType().ShouldSerialize())
            {
                actionArgumentValues.Add(JsonSerializer.Serialize(value));
            }
            else
            {
                actionArgumentValues.Add(value);
            }
        }

        var routeValuesHash = KeyHashGenerator.ToHash(string.Join(":", routeValues.Values));
        var actionArgumentsHash = KeyHashGenerator.ToHash(string.Join(":", actionArgumentValues));
        Assert.That(key, Is.EqualTo(KeyHashGenerator.ToHash(string.Join(":", [routeValuesHash, actionArgumentsHash]))));
    }
}