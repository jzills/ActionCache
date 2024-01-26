using Microsoft.AspNetCore.Mvc;
using ActionCache;
using ActionCache.Filters;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[ActionCacheFilterFactory(Namespace = "Test")]
public class SampleController : ControllerBase
{
    [HttpGet]
    [Route("/")]
    public IActionResult Get(
        [ActionCacheKey(Order = 1)]int id, 
        [ActionCacheKey(Order = 2)]DateTime date
    )
    {
        return Ok("Hooray!");
    }

    [HttpPost]
    [Route("/")]
    [ActionCacheEvictionFilterFactory(Namespace = "Test")]
    public IActionResult Post()
    {
        return Ok("Ok!");
    }
}