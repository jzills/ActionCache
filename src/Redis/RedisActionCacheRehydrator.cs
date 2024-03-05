using System.Text.Json;
using StackExchange.Redis;
using ActionCache.Common;
using ActionCache.Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using ActionCache.Attributes;

namespace ActionCache.Redis;

public class RehydrationResult
{
    public string Key { get; set; }
    public object Value { get; set; }
}

public class RedisActionCacheRehydrator : IActionCacheRehydrator
{
    protected readonly IDatabase Cache;
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;
    public RedisActionCacheRehydrator(
        IConnectionMultiplexer connectionMultiplexer,
        ActionCacheDescriptorProvider descriptorProvider
    ) => (Cache, DescriptorProvider) = (connectionMultiplexer.GetDatabase(), descriptorProvider);
    
    public async Task<List<RehydrationResult>> GetRehydrationResultsAsync(string @namespace)
    {
        var rehydrationDescriptor = new List<RehydrationResult>();
        var descriptorCollection = DescriptorProvider.GetControllerActionMethodInfo(@namespace);
        if (descriptorCollection.MethodInfos.Any())
        {
            foreach (var (route, methodInfo) in descriptorCollection.MethodInfos)
            {
                var controller = descriptorCollection.Controllers[route];
                var actionArgsJson = await Cache.SetMembersAsync($"ActionCache:Rehydration:{route}");
                var actionArgs = actionArgsJson.Select(arg => JsonSerializer.Deserialize<Dictionary<string, JsonElement>>((string)arg!));
                foreach (var actionArg in actionArgs)
                {
                    var actionValueConversions = new SortedList<int, object>();
                    foreach (var parameter in methodInfo.GetParameters())
                    {
                        var actionCacheAttribute = parameter.GetCustomAttribute<ActionCacheKeyAttribute>();
                        if (actionCacheAttribute is not null)
                        {
                            var actionValue = actionArg.First(arg => arg.Key == parameter.Name);
                            var actionValueConversion = actionValue.Value.Deserialize(parameter.ParameterType);
                            actionValueConversions.Add(actionCacheAttribute.Order, actionValueConversion);
                        }
                    }

                    var result = methodInfo.Invoke(controller, actionValueConversions.Values.ToArray());
                    if (result is OkObjectResult okObjectResult)
                    {
                        rehydrationDescriptor.Add(new RehydrationResult
                        {
                            Key = string.Join(":", actionValueConversions.Values),
                            Value = okObjectResult.Value
                        });
                    }
                }     
            }
        }

        return rehydrationDescriptor;
    }

    public Task SetAsync<TValue>(string key, TValue value)
    {
        return Cache.SetAddAsync(key, JsonSerializer.Serialize(value));
    }

    private bool TryGetValue(ParameterInfo parameterInfo)
    {
        var attribute = parameterInfo.GetCustomAttribute<ActionCacheKeyAttribute>();
        if (attribute is not null)
        {
            var actionValue = actionArg.First(arg => arg.Key == parameterInfo.Name);
            var actionValueConversion = actionValue.Value.Deserialize(parameterInfo.ParameterType);
            return (attribute.Order, actionValueConversion);
        }
    }
}