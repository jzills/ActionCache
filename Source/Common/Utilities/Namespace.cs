namespace ActionCache.Utilities;

public record class Namespace(string Value)
{
    protected const string Assembly = nameof(ActionCache);
    public string Create(string key) => $"{Assembly}:{Value}:{key}";
    public static implicit operator string(Namespace @this) => $"{Assembly}:{@this.Value}";
    public static implicit operator Namespace(string @namespace) => new Namespace(@namespace);
}