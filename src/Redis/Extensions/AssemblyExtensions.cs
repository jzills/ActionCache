using System.Reflection;

namespace ActionCache.Redis.Extensions;

public static class AssemblyExtensions
{
    private static Dictionary<string, string> _cache = new();

    public static bool TryGetResourceAsText(this Assembly source, string name, out string resource)
    {
        if (_cache.TryGetValue(name, out resource!))
        {
            return true;
        }
        else if (source.TryReadResourceStream(name, out resource))
        {
            _cache.Add(name, resource);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string GetResourceName(this Assembly source, string fileName)
    {
        var resourceName = source
            .GetManifestResourceNames()
            .FirstOrDefault(resource => resource.EndsWith(fileName));

        return resourceName ?? string.Empty;
    }

    private static bool TryReadResourceStream(this Assembly source, string name, out string resource)
    {
        try
        {
            var resourceName = source.GetResourceName(name);
            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                using var stream = source.GetManifestResourceStream(resourceName);
                using var reader = new StreamReader(stream!);
                resource = reader.ReadToEnd();
                return true;
            }
            else
            {
                resource = default!;
                return false;
            }
        }
        catch (Exception)
        {
            resource = default!;
            return false;
        }
    }
}