using Members.Application.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Members.Infrastructure.Security;

public sealed class EncryptedStringConverter : ValueConverter<string, string>
{
    public EncryptedStringConverter(IEncryptionService encryptionService)
        : base(
            v => encryptionService.Encrypt(v),
            v => encryptionService.Decrypt(v))
    {
    }
}
