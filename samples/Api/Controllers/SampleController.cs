using Microsoft.AspNetCore.Mvc;
using ActionCache.Attributes;
using Api.Models;

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
    public IActionResult Post(int id, DateTime date, SampleModel model)
    {
        return Ok($"The current time is {DateTime.Now}");
    }

    [HttpDelete]
    [Route("/")]
    [ActionCacheEviction(Namespace = "Namespace1, Namespace2")]
    public IActionResult Delete()
    {
        return Ok("Ok!");
    }
}