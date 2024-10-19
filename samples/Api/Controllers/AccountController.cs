using Microsoft.AspNetCore.Mvc;
using ActionCache.Attributes;
using Api.Database;
using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    [Route("{id}")]
    [ActionCache(Namespace = "Account:{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(account => account.Id == id);
        return Ok(account);
    }

    [HttpPut]
    [Route("{id}")]
    [ActionCacheRefresh(Namespace = "Account:{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] AccountModel model)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(account => account.Id == id);
        if (account is not null)
        {
            account.Name = model.Name;
            account.Code = model.Code;
            account.PhoneNumber = model.PhoneNumber;

            await _context.SaveChangesAsync();
            return Ok(account);
        }
        else
        {
            return BadRequest(); 
        }
    }

    [HttpPost]
    [Route("")]
    [ActionCacheRefresh(Namespace = "Account:{id}")]
    public async Task<IActionResult> Post([FromBody] AccountModel model)
    {
        var account = new Account
        {
            Name = model.Name,
            Code = model.Code,
            PhoneNumber = model.PhoneNumber
        };

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        return Ok(account.Id);
    }

    [HttpDelete]
    [Route("{id}")]
    [ActionCacheEviction(Namespace = "Account:{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(account => account.Id == id);
        if (account is not null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return Ok(id);
        }
        else
        {
            return BadRequest(); 
        }
    }
}