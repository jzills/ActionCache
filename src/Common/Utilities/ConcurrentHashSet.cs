using System.Collections.Concurrent;

namespace ActionCache.Utilities;

internal class ConcurrentHashSet<T> where T : notnull
{
    internal protected readonly ConcurrentDictionary<T, byte> Internal;

    internal ConcurrentHashSet()
    {
        Internal = new ConcurrentDictionary<T, byte>();
    }

    internal bool Add(T item) => Internal.TryAdd(item, 0);

    internal bool Remove(T item) => Internal.TryRemove(item, out _);

    internal bool ContainsKey(T key) => Internal.ContainsKey(key);

    internal int Count => Internal.Count;

    internal void Clear() => Internal.Clear();

    internal IEnumerable<T> ToList() => Internal.Keys;
}