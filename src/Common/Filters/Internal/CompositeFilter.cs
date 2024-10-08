namespace ActionCache.Common.Filters;

internal class CompositeFilter<T>
{
    internal protected readonly IReadOnlyCollection<T> Filters;

    internal CompositeFilter(IReadOnlyCollection<T> filters)
    {
        Filters = filters;
    }

    internal protected Task ExecuteCompositionAsync(Func<T, Task> filterActionAccessor) =>
        Task.WhenAll(Filters.Select(filter => 
            filterActionAccessor(filter)));
}