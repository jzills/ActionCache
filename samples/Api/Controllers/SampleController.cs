using Microsoft.AspNetCore.Mvc;
using ActionCache.Attributes;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[ActionCache(Namespace = "Namespace1")]
public class SampleController : ControllerBase
{
    [HttpPost]
    [Route("/")]
    [ActionCacheRehydration(Namespace = "Namespace1")]
    public IActionResult Post(
        [ActionCacheKey(Order = 1)]int id, 
        [ActionCacheKey(Order = 2)]DateTime date
    )
    {
        return Ok("Hooray!");
    }

    [HttpDelete]
    [Route("/")]
    [ActionCacheEviction(Namespaces = "Namespace1, Namespace2")]
    public IActionResult Delete()
    {
        return Ok("Ok!");
    }
}