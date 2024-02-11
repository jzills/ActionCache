using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace ActionCache.Common.Extensions.Internal;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddControllerInfo(this IServiceCollection services)
    {
        var managerServiceDescriptor = services.FirstOrDefault(service => 
            service.ServiceType == typeof(ApplicationPartManager));

        if (managerServiceDescriptor?.ImplementationInstance is ApplicationPartManager manager)
        {
            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);

            var controllerTypes = feature.Controllers.Select(controller => controller.AsType());
            if (controllerTypes?.Any() ?? false) 
            {
                // var controllers = controllerTypes
                //     .Where(controllerType => 
                //         controllerType.AssemblyQualifiedName is not null)
                //     .ToDictionary(
                //         controllerType => controllerType.AssemblyQualifiedName!, 
                //         controllerType => controllerType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                //             .Select(methodInfo => methodInfo.Name));

                foreach (var controller in controllerTypes)
                {
                    services.AddScoped(controller);
                }
            }
        }

        return services;
    }
}