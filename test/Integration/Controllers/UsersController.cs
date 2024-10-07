using ActionCache.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Integration.Controllers;

[Route("[controller]")]
public class UsersController : Controller
{
    [HttpGet("")]
    [ActionCache(Namespace = "Users")]
    public IActionResult Get() =>
        Ok(new object[]
        {
            new { Id = 1, Name = "Joshua" },
            new { Id = 2, Name = "Sam" },
            new { Id = 3, Name = "Izzy" },
            new { Id = 4, Name = "Vanessa" }
        });

    [HttpPost("")]
    [ActionCacheRefresh(Namespace = "Users")]
    public IActionResult Post() => Ok();

    [HttpDelete("")]
    [ActionCacheEviction(Namespace = "Users")]
    public IActionResult Delete() => Ok();
}