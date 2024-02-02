namespace ActionCache;

public interface IActionCache
{
    Task<TValue?> GetAsync<TValue>(string key);
    Task SetAsync<TValue>(string key, TValue? value);
    Task RemoveAsync(string key);
    Task RemoveAsync();
}