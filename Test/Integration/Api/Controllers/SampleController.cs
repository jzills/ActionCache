using Microsoft.AspNetCore.Mvc;
using ActionCache.Attributes;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[ActionCache(Namespace = "Namespace1")]
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
    [ActionCacheEviction(Namespaces = "Namespace1, Namespace2")]
    public IActionResult Post()
    {
        return Ok("Ok!");
    }
}