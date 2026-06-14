using System.Security.Cryptography;
using Members.Application.Common;

namespace Members.Infrastructure.Security;

public sealed class AesGcmEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public AesGcmEncryptionService(byte[] key)
    {
        if (key.Length != 32)
            throw new ArgumentException("Key must be 32 bytes (256 bits).", nameof(key));
        _key = key;
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        var plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        var cipherText = new byte[plainBytes.Length];

        RandomNumberGenerator.Fill(nonce);

        using var aes = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
        aes.Encrypt(nonce, plainBytes, cipherText, tag);

        var combined = new byte[nonce.Length + tag.Length + cipherText.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, combined, nonce.Length + tag.Length, cipherText.Length);

        return Convert.ToBase64String(combined);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        var combined = Convert.FromBase64String(cipherText);
        var nonceSize = AesGcm.NonceByteSizes.MaxSize;
        var tagSize = AesGcm.TagByteSizes.MaxSize;

        var nonce = combined[..nonceSize];
        var tag = combined[nonceSize..(nonceSize + tagSize)];
        var encrypted = combined[(nonceSize + tagSize)..];

        var plainBytes = new byte[encrypted.Length];

        using var aes = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
        aes.Decrypt(nonce, encrypted, tag, plainBytes);

        return System.Text.Encoding.UTF8.GetString(plainBytes);
    }
}
