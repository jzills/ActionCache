using ActionCache.Common.Enums;

namespace ActionCache.Exceptions;

public class FilterTypeNotSupportedException : NotSupportedException
{
    public FilterTypeNotSupportedException(FilterType filterType)
        : base($"The filter type \"{filterType}\" is not supported.")
    {
    }
}