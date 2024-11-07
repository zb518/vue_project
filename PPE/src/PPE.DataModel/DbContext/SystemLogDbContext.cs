using Microsoft.EntityFrameworkCore;

namespace PPE.DataModel;

public class SystemLogDbContext : DbContext
{
    public SystemLogDbContext(DbContextOptions<SystemLogDbContext> options) : base(options)
    {
    }

    public SystemLogDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Base_Log>(b =>
        {
            b.ToTable(nameof(Base_Log));
        });
    }

    public DbSet<Base_Log> Logs { get; set; }
}