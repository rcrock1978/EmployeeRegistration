namespace Members.Domain.Members;

public record Consent
{
    public bool ConsentGiven { get; init; }
    public bool Attestation { get; init; }
    public string SignatureName { get; init; } = null!;
    public DateTime SignedAt { get; init; }

    private Consent() { }

    public Consent(bool consentGiven, bool attestation, string signatureName, DateTime signedAt)
    {
        ConsentGiven = consentGiven;
        Attestation = attestation;
        SignatureName = signatureName;
        SignedAt = signedAt;
    }
}
