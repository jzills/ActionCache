using ActionCache.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Integration.Controllers;

[Route("[controller]")]
public class TeamsController : Controller
{
    [HttpGet("{id}")]
    [ActionCache(Namespace = "Teams:{id}")]
    public IActionResult Get(Guid id) => Ok(id);

    [HttpPut("{id}")]
    [ActionCacheRefresh(Namespace = "Teams:{id}")]
    public IActionResult Put(Guid id) => Ok(id);

    [HttpDelete("{id}")]
    [ActionCacheEviction(Namespace = "Teams:{id}")]
    public IActionResult Delete(Guid id) => Ok(id);
}