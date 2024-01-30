namespace ActionCache;

public interface IActionCache
{
    Task<TValue?> GetAsync<TValue>(string key);
    Task<bool> SetAsync<TValue>(string key, TValue? value);
    Task<bool> RemoveAsync(string key);
    Task<bool> RemoveAsync();
}