namespace Members.Infrastructure.Security;

public sealed class EncryptionOptions
{
    public const string SectionName = "Encryption";

    /// <summary>
    /// Base64-encoded 256-bit AES key. Must be 44 characters (32 bytes encoded).
    /// </summary>
    public string Base64EncodedKey { get; set; } = string.Empty;

    /// <summary>
    /// Dev-only: passphrase used to derive a key when Base64EncodedKey is empty.
    /// </summary>
    public string DevPassphrase { get; set; } = string.Empty;
}
