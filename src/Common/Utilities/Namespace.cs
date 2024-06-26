namespace ActionCache.Utilities;

public record class Namespace(string Value)
{
    protected const string Assembly = nameof(ActionCache);

    public string Create(string key) => Concat(Assembly, Value, key);

    public static implicit operator string(Namespace @this) => Concat(Assembly, @this.Value);
    public static implicit operator Namespace(string @namespace) => new Namespace(@namespace);

    private static string Concat(params string[] components) => string.Join(':', components);
}