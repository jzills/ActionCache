using System.Text.Json;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using ActionCache.Common;
using ActionCache.Common.Utilities;
using ActionCache.Redis.Extensions;
using System.Reflection;

namespace ActionCache.Redis;

public class RedisActionCacheRehydrator : IActionCacheRehydrator
{
    protected readonly IDatabase Cache;
    protected readonly ActionCacheDescriptorProvider DescriptorProvider;
    public RedisActionCacheRehydrator(
        IConnectionMultiplexer connectionMultiplexer,
        ActionCacheDescriptorProvider descriptorProvider
    ) => (Cache, DescriptorProvider) = (connectionMultiplexer.GetDatabase(), descriptorProvider);
    
    public async Task<IReadOnlyCollection<RehydrationResult>> GetRehydrationResultsAsync(string @namespace)
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
                    var actionValueConversions = new SortedList<int, object?>();
                    foreach (var parameter in methodInfo.GetParameters())
                    {
                        if (parameter.TryGetValue(actionArg, out var actionCacheAttribute))
                        {
                            actionValueConversions.Add(
                                actionCacheAttribute.Order, 
                                actionCacheAttribute.Value
                            );
                        }
                    }

                    if (TryGetRehydrationResult(
                            methodInfo, 
                            controller, 
                            actionValueConversions.Values.ToArray(), 
                            out var result
                    ))
                    {
                        rehydrationDescriptor.Add(result);
                    }
                }     
            }
        }

        return rehydrationDescriptor.AsReadOnly();
    }

    public Task SetAsync<TValue>(string key, TValue value)
    {
        return Cache.SetAddAsync(key, JsonSerializer.Serialize(value));
    }

    public bool TryGetRehydrationResult(
        MethodInfo? methodInfo, 
        object? controller, 
        object?[]? parameters, 
        out RehydrationResult value
    )
    {
        var result = methodInfo.Invoke(controller, parameters);
        if (result is OkObjectResult okObjectResult)
        {
            value = new RehydrationResult
            {
                Key = string.Join(":", parameters),
                Value = okObjectResult.Value
            };

            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }
}