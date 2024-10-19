using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActionCache.Common.Extensions.Internal;

internal static class StatusCodeResultExtensions
{
    internal static bool IsSuccessStatusCode(this StatusCodeResult result) =>
        result.StatusCode >= StatusCodes.Status200OK && 
        result.StatusCode <= StatusCodes.Status226IMUsed;
}