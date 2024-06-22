using System.Text.Json;
using StackExchange.Redis;
using ActionCache.Common;
using ActionCache.Common.Utilities;

namespace ActionCache.Redis;

internal class RedisActionCacheRehydratorInternal : RedisActionCache
{
    protected readonly RedisActionCacheRehydrator Rehydrator;
    public RedisActionCacheRehydratorInternal(
        RedisNamespace @namespace, 
        IDatabase cache,
        RedisActionCacheRehydrator rehydrator
    ) : base(@namespace, cache)
    {
        Rehydrator = rehydrator;
    }

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

internal class RedisActionCacheRehydrator : ActionCacheRehydrator
{
    protected readonly IDatabase Cache;
    public RedisActionCacheRehydrator(
        IConnectionMultiplexer connectionMultiplexer,
        ActionCacheDescriptorProvider descriptorProvider
    ) : base(descriptorProvider)
    {
        Cache = connectionMultiplexer.GetDatabase();
        ActionArgsAccessor = async @namespace =>
        {
            var actionArgsJson = await Cache.SetMembersAsync($"ActionCache:Rehydration:{@namespace}");
            var actionArgs = actionArgsJson.Select(arg => JsonSerializer.Deserialize<Dictionary<string, JsonElement>>((string)arg!));
            return actionArgs;
        };
    }
}