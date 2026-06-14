namespace Members.WebApi.Infrastructure;

public sealed class AuthOptions
{
    public const string SectionName = "Authentication";

    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string MetadataUrl { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;
    public int TokenValidationClockSkewMinutes { get; set; } = 5;

    /// <summary>
    /// When true, uses a development signing key instead of a real OIDC provider.
    /// </summary>
    public bool UseDevelopmentKey { get; set; } = false;

    /// <summary>
    /// The development signing key (at least 32 characters). Used only when UseDevelopmentKey is true.
    /// </summary>
    public string DevelopmentSigningKey { get; set; } = string.Empty;
}
