using Microsoft.EntityFrameworkCore;

namespace Members.Infrastructure.Persistence;

public class MembersDbContext : DbContext
{
    public MembersDbContext(DbContextOptions<MembersDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MembersDbContext).Assembly);
    }
}
