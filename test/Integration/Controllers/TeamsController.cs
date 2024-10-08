using ActionCache.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Integration.Controllers;

[Route("")]
public class TeamsController : Controller
{
    [HttpGet("{accountId}/teams/{teamId}")]
    [ActionCache(Namespace = "Teams:{accountId}")]
    public IActionResult GetMembers(Guid accountId, Guid teamId) => Ok(new
    {
        AccountId = accountId,
        TeamId = teamId
    });

    [HttpPut("{accountId}/teams/{teamId}/{id}")]
    [ActionCacheRefresh(Namespace = "Teams:{id}")]
    public IActionResult UpdateMember(Guid accountId, Guid teamId, Guid id) => Ok();

    [HttpDelete("{accountId}/teams/{teamId}/{id}")]
    [ActionCacheEviction(Namespace = "Teams:{id}")]
    public IActionResult DeleteMember(Guid accountId, Guid teamId, Guid id) => Ok();
}