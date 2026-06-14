using Members.Application.Common;
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
