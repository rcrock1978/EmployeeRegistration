using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Members.Infrastructure.Persistence;

public sealed class MembersDbContextFactory : IDesignTimeDbContextFactory<MembersDbContext>
{
    public MembersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MembersDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=optodev_members;Username=postgres;Password=postgres");

        return new MembersDbContext(optionsBuilder.Options);
    }
}
