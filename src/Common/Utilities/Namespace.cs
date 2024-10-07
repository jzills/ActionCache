namespace ActionCache.Utilities;

/// <summary>
/// Represents a namespace utility for string manipulations.
/// </summary>
public record class Namespace(string Value)
{
    protected const string Assembly = nameof(ActionCache);
    public string? ValueWithRouteTemplateParameters { get; set; }

    /// <summary>
    /// Creates a fully qualified namespace key.
    /// </summary>
    /// <param name="key">The key to append to the namespace.</param>
    /// <returns>A concatenated string with the assembly, namespace and key.</returns>
    public string Create(string key) => Concat(Assembly, ValueWithRouteTemplateParameters ?? Value, key);

    /// <summary>
    /// Allows implicit conversion of the Namespace instance to a string.
    /// </summary>
    /// <param name="this">The Namespace instance.</param>
    /// <returns>A concatenated string with the assembly and namespace value.</returns>
    public static implicit operator string(Namespace @this) => Concat(Assembly, @this.ValueWithRouteTemplateParameters ?? @this.Value);

    /// <summary>
    /// Allows implicit conversion of a string to a Namespace instance.
    /// </summary>
    /// <param name="namespace">The string to convert.</param>
    /// <returns>A new Namespace instance based on the string.</returns>
    public static implicit operator Namespace(string @namespace) => new Namespace(@namespace);

    /// <summary>
    /// Joins multiple string components with a colon separator.
    /// </summary>
    /// <param name="components">The string components to concatenate.</param>
    /// <returns>The concatenated string.</returns>
    private static string Concat(params string[] components) => string.Join(':', components);
}