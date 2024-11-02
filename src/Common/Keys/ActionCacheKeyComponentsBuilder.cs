using System.Collections.Specialized;
using System.Web;
using ActionCache.Common.Extensions.Internal;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Keys;

public class ActionCacheKeyComponentsBuilder
{
    protected readonly NameValueCollection NameValues;
    protected readonly KeyCryptoGenerator KeyGenerator = new();

    public ActionCacheKeyComponentsBuilder(string value)
    {
        NameValues = HttpUtility.ParseQueryString(KeyGenerator.Decrypt(value));
    }             

    public ActionCacheKeyComponents Build() =>
        new ActionCacheKeyComponents 
        { 
            RouteValues     = NameValues.ParseValueAsJson<RouteValueDictionary>(ActionCacheKeyComponents.RouteValuesKey), 
            ActionArguments = NameValues.ParseValueAsJson<Dictionary<string, dynamic>>(ActionCacheKeyComponents.ActionArgumentsKey)
        };
}