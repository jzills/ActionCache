namespace ActionCache.Redis;

public class RehydrationResult
{
    public string? Key { get; set; }
    public object? Value { get; set; }
}