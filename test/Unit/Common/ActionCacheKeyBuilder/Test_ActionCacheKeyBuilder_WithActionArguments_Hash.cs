using Unit.TestUtiltiies.Data;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using ActionCache.Common.Keys;
using ActionCache.Common.Serialization;

namespace Unit.Common;

[TestFixture]
public class Test_ActionCacheKeyBuilder_WithActionArguments_Hash
{
    [Test]
    [TestCaseSource(typeof(TestData), nameof(TestData.GetControllerDescriptors))]
    public void Test(
        IEnumerable<ControllerParameterDescriptor> _, 
        RouteValueDictionary routeValues, 
        Dictionary<string, object> actionArguments
    )
    {
        var key = new ActionCacheKeyBuilder()
            .WithRouteValues(routeValues)
            .WithActionArguments(actionArguments)
            .Build();

        var decryptedKey = new KeyCryptoGenerator().Decrypt(key);
        Assert.That(decryptedKey, Is.EqualTo($"{ActionCacheKeyComponents.RouteValuesKey}={CacheJsonSerializer.Serialize(routeValues)}&{ActionCacheKeyComponents.ActionArgumentsKey}={CacheJsonSerializer.Serialize(actionArguments)}"));
    }
}