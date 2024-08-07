using System.Text;
using System.Text.Json;
using ActionCache.Attributes;
using Newtonsoft.Json;

namespace ActionCache.Common.Extensions;

internal static class ActionCacheKeyAttributeExtensions
{
    internal static IEnumerable<object> GetArguments(
        this IReadOnlyDictionary<string, ActionCacheKeyAttribute> source,
        IDictionary<string, object> args
    )
    {
        var mappedArgs = new List<object>();
        foreach (var attribute in source.OrderBy(attribute => attribute.Value.Order))
        {
            var arg = args[attribute.Key];
            var argType = arg.GetType();
            if (argType.IsClass)
            {
                // TODO: Add configuration option to use base64 encoding class key elements
                //mappedArgs.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(arg))));
                mappedArgs.Add(JsonConvert.SerializeObject(arg));
            }
            else
            {
                mappedArgs.Add(arg);
            }
        }

        return mappedArgs;
    }
}