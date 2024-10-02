namespace ActionCache.Common;

/// <summary>
/// Represents the result of a rehydration process, storing the key and the value retrieved.
/// </summary>
internal class RefreshResult
{
    /// <summary>
    /// Gets or sets the key related to the rehydrated value.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the value that was rehydrated.
    /// </summary>
    public object? Value { get; set; }
}