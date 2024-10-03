using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions.Internal;

/// <summary>
/// Extension methods for IServiceCollection to support additional functionality
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds scoped services for controller types discovered from ApplicationPartManager.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection for chaining.</returns>
    public static IServiceCollection AddControllerInfo(this IServiceCollection services)
    {
        var managerServiceDescriptor = services.FirstOrDefault(service => 
            service.ServiceType == typeof(ApplicationPartManager));

        if (managerServiceDescriptor?.ImplementationInstance is ApplicationPartManager manager)
        {
            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);

            var controllerTypes = feature.Controllers.Select(controller => controller.AsType());
            if (controllerTypes.Some()) 
            {
                foreach (var controller in controllerTypes)
                {
                    services.AddScoped(controller);
                }
            }
        }

        return services;
    }
}