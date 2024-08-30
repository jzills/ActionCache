using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ActionCache.Common.Extensions.Internal
{
    /// <summary>
    /// Provides extension methods for <see cref="ActionDescriptorCollection"/>.
    /// </summary>
    public static class ActionDescriptorCollectionExtensions
    {
        /// <summary>
        /// Attempts to get controller action descriptors that have an ActionCache attribute in the specified namespace.
        /// </summary>
        /// <param name="source">The collection of action descriptors.</param>
        /// <param name="namespace">The namespace to filter by.</param>
        /// <param name="descriptors">The filtered controller action descriptors that match the namespace.</param>
        /// <returns>true if any descriptors are found; otherwise, false.</returns>
        public static bool TryGetControllerActionDescriptors(
            this ActionDescriptorCollection source, 
            string @namespace, 
            out IEnumerable<ControllerActionDescriptor> descriptors
        )
        {
            descriptors = source.Items.OfType<ControllerActionDescriptor>()
                .Where(descriptor => descriptor.MethodInfo.HasActionCacheAttribute(@namespace));

            return descriptors.Any();
        }
    }
}