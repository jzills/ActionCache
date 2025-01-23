using ActionCache.AzureCosmos.Exceptions;
using ActionCache.Common.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.AzureCosmos.Extensions;

/// <summary>
/// Provides extension methods for adding Action Cache Azure Cosmos Db implementation to IServiceCollection.
/// </summary>
internal static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds Action Cache Azure Cosmos Db services with configuration options to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configureOptions">The configuration options for the Azure Cosmos Db cache.</param>
    /// <returns>The IServiceCollection with added Action Cache Azure Cosmos Db services.</returns>
    internal static IServiceCollection AddActionCacheAzureCosmos(
        this IServiceCollection services,
        Action<AzureCosmosCacheOptions> configureOptions
    ) 
    {
        var options = new AzureCosmosCacheOptions();
        configureOptions.Invoke(options);

        if (string.IsNullOrEmpty(options.ConnectionString))
        {
            throw new MissingConnectionStringException();
        }

        if (string.IsNullOrEmpty(options.DatabaseId))
        {
            throw new MissingDatabaseIdException();
        }

        return services
            .AddActionCacheCommon()
            .AddScoped<AzureCosmosActionCacheProvider>()
            .AddScoped<IActionCacheFactory, AzureCosmosActionCacheFactory>(serviceProvider => 
                serviceProvider.GetRequiredService<AzureCosmosActionCacheProvider>()
                    .CreateAsync(options.DatabaseId)
                    .GetAwaiter()
                    .GetResult()
            )
            .AddSingleton(serviceProvider => 
                new CosmosClient(
                    options.ConnectionString, 
                    options.CosmosClientOptions));
    }
}