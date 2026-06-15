using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class ConsentTests
{
    private static readonly DateTime Now = DateTime.UtcNow;

    [Fact]
    public void Create_WithGivenConsent_SetsProperties()
    {
        var consent = new Consent(true, true, "Juan Dela Cruz", Now);

        Assert.True(consent.ConsentGiven);
        Assert.True(consent.Attestation);
        Assert.Equal("Juan Dela Cruz", consent.SignatureName);
        Assert.Equal(Now, consent.SignedAt);
    }

    [Fact]
    public void Create_WithDeclinedConsent_SetsProperties()
    {
        var consent = new Consent(false, false, "", Now);

        Assert.False(consent.ConsentGiven);
        Assert.False(consent.Attestation);
        Assert.Equal("", consent.SignatureName);
    }
}
