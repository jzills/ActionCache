namespace ActionCache.Utilities;

public record struct Namespace(string Value)
{
    private const string _assembly = nameof(ActionCache);
    public string Create(string key) => $"{_assembly}:{Value}:{key}";
    public static implicit operator string(Namespace @this) => $"{_assembly}:{@this.Value}";
}