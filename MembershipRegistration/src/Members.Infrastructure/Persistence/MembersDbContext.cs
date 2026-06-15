using Members.Application.Common;
using Members.Domain.AdminUsers;
using Members.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Members.Infrastructure.Security;

namespace Members.Infrastructure.Persistence;

public class MembersDbContext : DbContext
{
    private readonly IEncryptionService? _encryptionService;

    public MembersDbContext(DbContextOptions<MembersDbContext> options, IEncryptionService? encryptionService = null)
        : base(options)
    {
        _encryptionService = encryptionService;
    }

    public DbSet<Member> Members => Set<Member>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MembersDbContext).Assembly);

        if (_encryptionService is not null)
        {
            modelBuilder.ApplySensitiveFieldEncryption(_encryptionService);
        }
    }
}
