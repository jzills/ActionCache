using System.Collections.Concurrent;

namespace ActionCache.Utilities;

public class ConcurrentHashSet<T>
{
    private readonly ConcurrentDictionary<T, byte> _dictionary;

    public ConcurrentHashSet()
    {
        _dictionary = new ConcurrentDictionary<T, byte>();
    }

    public bool Add(T item) => _dictionary.TryAdd(item, 0);

    public bool Remove(T item) => _dictionary.TryRemove(item, out _);

    public bool ContainsKey(T key) => _dictionary.ContainsKey(key);

    public int Count => _dictionary.Count;

    public void Clear() => _dictionary.Clear();

    public IEnumerable<T> ToList() => _dictionary.Keys;
}