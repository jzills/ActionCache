using System.Net;
using ActionCache.AzureCosmos.Exceptions;
using ActionCache.Common;
using ActionCache.Common.Caching;
using ActionCache.Common.Extensions;
using ActionCache.Utilities;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            .AddScoped<IActionCacheFactory, AzureCosmosActionCacheFactory>(serviceProvider => 
            {
                var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
                var entryOptions = serviceProvider.GetRequiredService<IOptions<ActionCacheEntryOptions>>();
                var refreshProvider = serviceProvider.GetRequiredService<IActionCacheRefreshProvider>();

                var response = cosmosClient.CreateDatabaseIfNotExistsAsync(options.DatabaseId).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.OK || 
                    response.StatusCode == HttpStatusCode.Created
                )
                {
                    return new AzureCosmosActionCacheFactory(
                        response.Database.GetContainer(Namespace.Assembly),
                        entryOptions,
                        refreshProvider
                    );
                }
                else
                {
                    throw new AzureCosmosDatabaseNotFoundOrCreated(response);
                }
            })
            .AddSingleton(serviceProvider => 
                new CosmosClient(
                    options.ConnectionString, 
                    options.CosmosClientOptions));
    }
}