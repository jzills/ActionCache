using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Filters;

internal class CompositeResultFilter : CompositeFilter<IAsyncResultFilter>, IAsyncResultFilter
{
    internal CompositeResultFilter(IReadOnlyCollection<IAsyncResultFilter> filters) 
        : base(filters)
    {
    }

    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) => 
        ExecuteCompositionAsync(filter => 
            filter.OnResultExecutionAsync(context, next));
}