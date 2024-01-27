// namespace ActionCache;

// public class ActionCacheNamespaced : IActionCache
// {
//     protected readonly string Namespace;
//     protected readonly IActionCache Cache;

//     public ActionCacheNamespaced(
//         string @namespace,
//         IActionCache cache
//     ) => (Namespace, Cache) = (@namespace, cache);

//     public Task<TValue?> GetAsync<TValue>(string key) =>
//         Cache.GetAsync<TValue?>($"{Namespace}:{key}");

//     public Task RemoveAsync(string key) =>
//         Cache.RemoveAsync($"{Namespace}:{key}");

//     public Task RemoveAsync() =>
//         Cache.RemoveAsync();

//     public Task SetAsync<TValue>(string key, TValue? value) =>
//         Cache.SetAsync($"{Namespace}:{key}", value);
// }