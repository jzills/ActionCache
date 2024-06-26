using Microsoft.AspNetCore.Mvc;
using ActionCache.Attributes;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleController : ControllerBase
{
    [HttpPut]
    [Route("/")]
    public IActionResult Put()
    {
        return Ok("My result!");
    }

    [HttpPost]
    [Route("/")]
    [ActionCache(Namespace = "Namespace1")]
    public IActionResult Post(
        [ActionCacheKey(Order = 1)]int id, 
        [ActionCacheKey(Order = 2)]DateTime date
    )
    {
        return Ok($"The current time is {DateTime.Now}");
    }

    [HttpDelete]
    [Route("/")]
    [ActionCacheEviction(Namespaces = "Namespace1, Namespace2")]
    public IActionResult Delete()
    {
        return Ok("Ok!");
    }
}