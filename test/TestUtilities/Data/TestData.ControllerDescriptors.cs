using ActionCache.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq.Expressions;
using System.Reflection;
using Unit.TestUtiltiies.Extensions;

namespace Unit.TestUtiltiies.Data;

public static partial class TestData
{
    public static IEnumerable<TestCaseData> GetControllerDescriptors() =>
    [
        CreateTestCase<SampleController, IActionResult>(controller => controller.OneParameter(1), new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" }, 
            { "action", "someAction" }
        }),
        CreateTestCase<SampleController, IActionResult>(controller => controller.TwoParametersWithEqualOrder(1, "test"), new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" }, 
            { "action", "someAction" }
        }),
        CreateTestCase<SampleController, IActionResult>(controller => controller.TwoParametersAscendingOrder(1, "test"), new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" }, 
            { "action", "someAction" }
        }),
        CreateTestCase<SampleController, IActionResult>(controller => controller.TwoParametersDescendingOrder(1, "test"), new RouteValueDictionary
        {
            { "area", "someArea" },
            { "controller", "someController" }, 
            { "action", "someAction" }
        })
    ];

    private static TestCaseData CreateTestCase<TController, TResult>(
        Expression<Func<TController, TResult>> actionExpression, 
        RouteValueDictionary routeValues
    ) where TController : Controller
    {
        var controllerActionDescriptorsWithValues = ControllerExtensions.GetControllerActionDescriptorsWithValues(actionExpression);
        var actionArguments = controllerActionDescriptorsWithValues
            .Where(controllerActionDescriptor => controllerActionDescriptor.ParameterDescriptor.ParameterInfo.GetCustomAttribute<ActionCacheKeyAttribute>() is not null)
            .OrderBy(controllerActionDescriptor => controllerActionDescriptor.ParameterDescriptor.ParameterInfo.GetCustomAttribute<ActionCacheKeyAttribute>()!.Order)
            .ToDictionary(
                controllerActionDescriptor => controllerActionDescriptor.ParameterDescriptor.Name, 
                controllerActionDescriptor => controllerActionDescriptor.ParameterValue
            );

        return new TestCaseData(
            controllerActionDescriptorsWithValues.Select(controllerActionDescriptorsWithValue => controllerActionDescriptorsWithValue.ParameterDescriptor),
            routeValues,
            actionArguments
        );
    }

    private class SampleController : Controller
    {
        public IActionResult OneParameter([ActionCacheKey] int id) => Ok();
        public IActionResult TwoParametersWithEqualOrder([ActionCacheKey(Order = 1)] int id, [ActionCacheKey(Order = 1)] string email) => Ok();
        public IActionResult TwoParametersAscendingOrder([ActionCacheKey(Order = 1)] int id, [ActionCacheKey(Order = 2)] string email) => Ok();
        public IActionResult TwoParametersDescendingOrder([ActionCacheKey(Order = 2)] int id, [ActionCacheKey(Order = 1)] string email) => Ok();
    }
}