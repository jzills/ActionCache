using System.Text.Json;
using System.Reflection;
using StackExchange.Redis;
using ActionCache.Utilities;
using ActionCache.Redis.Extensions;

namespace ActionCache.Redis;

public class RedisKeySetActionCache : RedisActionCache
{
    protected readonly RedisKey NamespaceKey;

    public RedisKeySetActionCache(
        Namespace @namespace, 
        IDatabase cache
    ) : base(@namespace, cache)
    {
        NamespaceKey = new RedisKey(@namespace);
    }

    public override async Task<bool> RemoveAsync(string key)
    {
        var isSuccessful = await base.RemoveAsync(key);
        if (isSuccessful)
        {
            return await Cache.SetRemoveAsync(NamespaceKey, key, CommandFlags.FireAndForget);
        }
        else
        {
            return false;
        }
    }

    public override async Task<bool> RemoveAsync()
    {
        var assembly = Assembly.GetExecutingAssembly();
        if (assembly.TryGetResourceAsText("UnlinkNamespaceWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, new[] { NamespaceKey }, null, CommandFlags.FireAndForget);
            return true;
        }
        else
        {
            var members = await Cache.SetMembersAsync(NamespaceKey);
            if (members.Any())
            {
                var membersTasksResult = new List<(string Key, Task<bool> Result)>(members.Length);
                foreach (var member in members)
                {
                    var memberKey = Namespace.Create(member!);
                    membersTasksResult.Add((memberKey, Cache.KeyDeleteAsync(memberKey)));
                }

                var setRemoveTasksResult = new List<Task<bool>>();
                foreach (var (key, result) in membersTasksResult)
                {
                    var isSuccessful = await result;
                    if (isSuccessful)
                    {
                        setRemoveTasksResult.Add(
                            Cache.SetRemoveAsync(NamespaceKey, key, CommandFlags.FireAndForget));
                    }
                }

                await Task.WhenAll(setRemoveTasksResult);
                
                var isSuccessfulExecution = true;
                foreach (var result in setRemoveTasksResult)
                {
                    var isSuccessful = await result;
                    if (!isSuccessful)
                    {
                        isSuccessfulExecution = false;
                        break;
                    }
                }

                return isSuccessfulExecution;
            }
            else
            {
                // Nothing to do here...
                return true;
            }
        }
    }

    public override async Task<bool> SetAsync<TValue>(string key, TValue? value) where TValue : default
    {
        var assembly = Assembly.GetExecutingAssembly();
        if (assembly.TryGetResourceAsText("SetJsonWithKeySet.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, 
                new[] { NamespaceKey, (RedisKey)key }, 
                new[] { (RedisValue)JsonSerializer.Serialize(value) }, 
                CommandFlags.FireAndForget
            );

            return true;
        }
        else
        {
            var isSuccessful = await base.SetAsync(key, value);
            if (isSuccessful)
            {
                return await Cache.SetAddAsync(NamespaceKey, key, CommandFlags.FireAndForget);
            }
            else
            {
                return false;
            }
        }
    }
}

public class RedisActionCache : IActionCache
{
    protected readonly Namespace Namespace;
    protected readonly IDatabase Cache;

    public RedisActionCache(
        Namespace @namespace,
        IDatabase cache
    ) 
    {
        Namespace = @namespace;
        Cache = cache;
    }

    public Task<TValue?> GetAsync<TValue>(string key)
    {
        var jsonValue = Cache.StringGet(Namespace.Create(key));
        if (!string.IsNullOrWhiteSpace(jsonValue))
        {
            var value = JsonSerializer.Deserialize<TValue>(jsonValue!);
            return Task.FromResult(value);
        }
        else
        {
            return Task.FromResult<TValue?>(default);
        }
    }

    public virtual Task<bool> RemoveAsync(string key) =>
        Cache.KeyDeleteAsync(Namespace.Create(key));

    public virtual async Task<bool> RemoveAsync()
    {
        var assembly = Assembly.GetExecutingAssembly();
        if (assembly.TryGetResourceAsText("UnlinkNamespace.lua", out var script))
        {
            await Cache.ScriptEvaluateAsync(script, null, new[] 
            { 
                new RedisValue($"{Namespace}:*") 
            });
        }
        else
        {
            // TODO: Use IConnectionMultiplexer to get all servers,
            // iterate through servers + scan keys and remove 
            // namespaces as fallback
            // REF: https://stackexchange.github.io/StackExchange.Redis/KeysScan.html 
        }

        return true;
    }

    public virtual Task<bool> SetAsync<TValue>(string key, TValue? value) =>
        Cache.StringSetAsync(
            Namespace.Create(key), 
            JsonSerializer.Serialize(value));
}