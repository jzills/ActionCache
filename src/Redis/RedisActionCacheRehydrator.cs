using System.Text.Json;
using StackExchange.Redis;
using ActionCache.Common;
using ActionCache.Common.Utilities;

namespace ActionCache.Redis;

public class RedisActionCacheRehydrator : ActionCacheRehydrator
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