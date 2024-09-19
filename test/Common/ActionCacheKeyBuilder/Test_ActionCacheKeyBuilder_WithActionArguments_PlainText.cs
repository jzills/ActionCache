using System.Reflection;
using ActionCache.Attributes;
using ActionCache.Common.Utilities;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheKeyBuilder_WithActionArguments_PlainText
{
    [Test]
    public async Task Test()
    {
        var methodInfo = typeof(SampleController).GetMethod(nameof(SampleController.SampleAction));
        var parameterDescriptors = methodInfo.GetParameters().Select(parameter => new ControllerParameterDescriptor
        {
            Name = parameter.Name,
            ParameterInfo = parameter,
            ParameterType = parameter.ParameterType
        }).ToArray();

        var actionDescriptor = new ActionDescriptor
        {
            Parameters = parameterDescriptors
        };

        var routeData = new RouteData(new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" }, 
            { "action", "someAction" }
        });

        var actionArguments = new Dictionary<string, object> { { "id", 1 } };

        var key = new ActionCacheKeyBuilder(actionDescriptor, useHashForKeyComponents: false)
            .WithRouteData(routeData)
            .WithActionArguments(actionArguments)
            .Build();

        Assert.That(key, Is.EqualTo("someArea:someController:someAction:1"));
    }

    public class SampleController
    {
        public void SampleAction([ActionCacheKey]int id) { }
    }
}