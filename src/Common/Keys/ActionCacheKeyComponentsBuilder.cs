using System.Collections.Specialized;
using System.Text.Json;
using System.Web;
using ActionCache.Common.Extensions.Internal;
using Microsoft.AspNetCore.Routing;

namespace ActionCache.Common.Keys;

public class ActionCacheKeyComponentsBuilder
{
    protected readonly NameValueCollection NameValues;

    public ActionCacheKeyComponentsBuilder(string value)
    {
        NameValues = HttpUtility.ParseQueryString(new KeyCryptoGenerator().Decrypt(value));
    }             

    public ActionCacheKeyComponents Build() =>
        new ActionCacheKeyComponents 
        { 
            RouteValues     = NameValues.ParseValueAsJson<RouteValueDictionary>("route"), 
            ActionArguments = NameValues.ParseValueAsJson<Dictionary<string, dynamic>>("args")
        };
}