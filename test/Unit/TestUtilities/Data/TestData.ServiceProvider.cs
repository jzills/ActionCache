using ActionCache.Common.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.TestUtiltiies.Data;

public static partial class TestData
{
    public static IEnumerable<IServiceProvider> GetServiceProviders() =>
        GetMemoryCacheServiceProvider().Concat(
            GetRedisCacheServiceProvider());

    public static IEnumerable<IServiceProvider> GetMemoryCacheServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddMvc();
        services.AddActionCache(options => 
        {
            options.UseEntryOptions(entryOptions => 
            {
                entryOptions.AbsoluteExpiration = TimeSpan.FromMinutes(15);
                entryOptions.SlidingExpiration = TimeSpan.FromMinutes(5);
            });
            options.UseMemoryCache(options => { });
        });

        var server = new TestServer(services.BuildServiceProvider());

        return [server.Services];
    }

    public static IEnumerable<IServiceProvider> GetRedisCacheServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddMvc();
        services.AddActionCache(options => 
        {
            options.UseEntryOptions(entryOptions => { });
            options.UseRedisCache(options => options.Configuration = "127.0.0.1:6379");
        });

        var server = new TestServer(services.BuildServiceProvider());

        return [server.Services];
    }
}