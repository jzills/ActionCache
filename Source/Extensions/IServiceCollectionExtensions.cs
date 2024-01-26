using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Extensions;

public class ActionCacheOptions
{
    public string Namespace { get; set; } = string.Empty;
}

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActionCache(this IServiceCollection services, Action<ActionCacheOptions> configureOptions)
    {
        // var options = new ActionCacheOptions();
        // configureOptions.Invoke(options);

        // ArgumentNullException.ThrowIfNullOrWhiteSpace(options.Namespace);

        return services
            .AddSingleton<MemoryCacheExpirationTokens>();

            // TODO: Move to ActionCacheFactory
            // Create objects in the FilterFactory
            // .AddScoped<IActionCache, MemoryActionCache>(provider => 
            // {
            //     var cancellationTokenSources = provider.GetRequiredService<MemoryCacheExpirationTokens>();
            //     if (cancellationTokenSources.TryGetOrAdd(options.Namespace, out var cancellationTokenSource))
            //     {
            //         return new MemoryActionCache(
            //             options.Namespace, 
            //             provider.GetRequiredService<IMemoryCache>(),
            //             cancellationTokenSource
            //         );
            //     }
            //     else
            //     {
            //         throw new NotImplementedException();
            //     }
            // });
    } 
}