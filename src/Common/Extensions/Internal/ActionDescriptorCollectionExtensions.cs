using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ActionCache.Common.Extensions.Internal;

public static class ActionDescriptorCollectionExtensions
{
    public static bool TryGetControllerActionDescriptors(
        this ActionDescriptorCollection source, 
        string @namespace, 
        out IEnumerable<ControllerActionDescriptor> descriptors
    )
    {
        descriptors = source.Items.
            OfType<ControllerActionDescriptor>()
            .Where(descriptor => descriptor.MethodInfo.HasActionCacheAttribute(@namespace));

        return descriptors.Any();
    }
}