using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }
}