using Members.Application.Common;
using Members.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Members.Infrastructure.Security;

public static class EncryptionModelBuilderExtensions
{
    private static readonly string[] SensitiveFields =
    [
        nameof(GovernmentIds.Tin),
        nameof(GovernmentIds.Sss),
        nameof(PrimaryIdentification.Number),
    ];

    public static void ApplySensitiveFieldEncryption(this ModelBuilder modelBuilder, IEncryptionService encryptionService)
    {
        var converter = new ValueConverter<string, string>(
            v => encryptionService.Encrypt(v),
            v => encryptionService.Decrypt(v));

        modelBuilder.Entity<Member>(entity =>
        {
            entity.OwnsOne(x => x.GovernmentIds, gi =>
            {
                gi.Property(nameof(GovernmentIds.Tin)).HasConversion(converter);
                gi.Property(nameof(GovernmentIds.Sss)).HasConversion(converter);
            });

            entity.OwnsOne(x => x.PrimaryIdentification, pi =>
            {
                pi.Property(nameof(PrimaryIdentification.Number)).HasConversion(converter);
            });
        });
    }
}
