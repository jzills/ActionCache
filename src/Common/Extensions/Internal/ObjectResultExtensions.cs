using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActionCache.Common.Extensions.Internal;

internal static class ObjectResultExtensions
{
    internal static bool IsSuccessStatusCode(this ObjectResult result) =>
        result.StatusCode >= StatusCodes.Status200OK && 
        result.StatusCode <= StatusCodes.Status226IMUsed;
}