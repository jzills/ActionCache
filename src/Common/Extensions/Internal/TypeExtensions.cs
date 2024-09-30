namespace ActionCache.Common.Extensions;

/// <summary>
/// Provides extension methods for handling types.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Determines if a type should be serialized.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>A bool indicating if the type should be serialized.</returns>
    internal static bool ShouldSerialize(this Type type) => 
        type != typeof(string) && type.IsClass;
}