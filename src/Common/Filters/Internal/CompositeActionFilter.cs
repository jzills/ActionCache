using Microsoft.AspNetCore.Mvc.Filters;

namespace ActionCache.Common.Filters;

internal class CompositeActionFilter : CompositeFilter<IAsyncActionFilter>, IAsyncActionFilter
{
    internal CompositeActionFilter(IReadOnlyCollection<IAsyncActionFilter> filters) 
        : base(filters)
    {
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) =>
        ExecuteCompositionAsync(filter => 
            filter.OnActionExecutionAsync(context, next));
}