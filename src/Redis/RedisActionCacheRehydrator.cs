using System.Text.Json;
using ActionCache.Common;
using ActionCache.Common.Utilities;
using StackExchange.Redis;

namespace ActionCache.Redis;

/// <summary>
/// RedisActionCacheRehydratorInternal class for rehydrating cached values in Redis.
/// </summary>
internal class RedisActionCacheRehydratorInternal : RedisActionCache
{
    /// <summary>
    /// Constructor for RedisActionCacheRehydratorInternal class.
    /// </summary>
    /// <param name="namespace">The RedisNamespace.</param>
    /// <param name="cache">The IDatabase cache.</param>
    /// <param name="rehydrator">The RedisActionCacheRehydrator instance.</param>
    public RedisActionCacheRehydratorInternal(
        RedisNamespace @namespace, 
        IDatabase cache,
        RedisActionCacheRehydrator rehydrator
    ) : base(@namespace, cache)
    {
        //Rehydrator = rehydrator;
    }

    /// <summary>
    /// Method to set rehydration values asynchronously.
    /// </summary>
    /// <param name="keySuffix">The key suffix.</param>
    /// <param name="values">The dictionary of values.</param>
    public async Task SetRehydrationValuesAsync(
        string keySuffix, 
        IDictionary<string, object?> values
    )
    {
        var setKey = $"ActionCache:{Namespace}:Rehydration";
        var setMemberKey = $"ActionCache:{Namespace}:Rehydration:{keySuffix}:{string.Join(":", values)}";
        await Cache.SetAddAsync(setKey, setMemberKey);
        await Cache.SetAddAsync(setMemberKey, JsonSerializer.Serialize(values));
    } 
}

/// <summary>
/// RedisActionCacheRehydrator class for rehydrating cached values in Redis.
/// </summary>
internal class RedisActionCacheRehydrator : ActionCacheRehydrator
{
    /// <summary>
    /// Constructor for RedisActionCacheRehydrator class.
    /// </summary>
    /// <param name="connectionMultiplexer">The IConnectionMultiplexer instance.</param>
    /// <param name="descriptorProvider">The ActionCacheDescriptorProvider instance.</param>
    public RedisActionCacheRehydrator(
        IConnectionMultiplexer connectionMultiplexer,
        ActionCacheDescriptorProvider descriptorProvider
    ) : base(descriptorProvider)
    {
        // Cache = connectionMultiplexer.GetDatabase();
        // ActionArgsAccessor = async @namespace =>
        // {
        //     var actionArgsJson = await Cache.SetMembersAsync($"ActionCache:Rehydration:{@namespace}");
        //     var actionArgs = actionArgsJson.Select(arg => JsonSerializer.Deserialize<Dictionary<string, JsonElement>>((string)arg!));
        //     return actionArgs;
        // };
    }
}