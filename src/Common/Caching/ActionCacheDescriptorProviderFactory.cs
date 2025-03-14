using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Caching;

/// <summary>
/// Factory for creating instances of <see cref="IActionCacheDescriptorProvider"/>.
/// </summary>
public class ActionCacheDescriptorProviderFactory
{
    /// <summary>
    /// The service provider used to resolve dependencies.
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionCacheDescriptorProviderFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    public ActionCacheDescriptorProviderFactory(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    /// <summary>
    /// Creates an instance of <see cref="IActionCacheDescriptorProvider"/>.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="ActionCacheDescriptorProvider"/> if <see cref="IActionDescriptorCollectionProvider"/> is available;
    /// otherwise, returns an instance of <see cref="ActionCacheDescriptorProviderNull"/>.
    /// </returns>
    public IActionCacheDescriptorProvider Create()
    {
        var descriptorProvider = ServiceProvider.GetService<IActionDescriptorCollectionProvider>();
        return descriptorProvider is null ?
            new ActionCacheDescriptorProviderNull() :
            new ActionCacheDescriptorProvider(ServiceProvider, descriptorProvider);
    }
}
