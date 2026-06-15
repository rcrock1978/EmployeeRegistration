using Members.Application.Features.Members.RegisterMember;

namespace Members.Application.UnitTests.Features.Members.RegisterMember;

public sealed class RegisterMemberCommandValidatorTests
{
    private readonly RegisterMemberCommandValidator _sut = new();

    private static RegisterMemberCommand CreateValidCommand() => new(
        PersonalInfo: new("Mr.", "Juan", null, "Dela Cruz", null, null,
            new DateTime(1990, 5, 12, 0, 0, 0, DateTimeKind.Utc),
            "Manila", "Philippines", "Filipino", "Male", "Single", null, "College Graduate", 0),
        ContactInfo: new("juan@email.com", "+639170000000"),
        RelatedPersons: new(null, null, null),
        GovernmentIds: new("123-456-789-000", "01-2345678-9"),
        PrimaryId: new("Passport", "P1234567A",
            new DateTime(2020, 1, 10, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2030, 1, 9, 0, 0, 0, DateTimeKind.Utc),
            "Philippines"),
        CurrentAddress: new("123 Rizal St.", "Manila", "1000", "Poblacion", null,
            "Metro Manila", "Philippines", "Owner",
            new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
        PermanentAddress: new(true, null),
        EmergencyContact: new("Maria Santos", "Spouse", "+639170000001"),
        Employment: new("RNF", "OPTODEV Inc.", "EMP-001", 45000m, "Monthly", "Technician",
            new DateTime(2019, 6, 1, 0, 0, 0, DateTimeKind.Utc), null),
        Consent: new(true, true, "Juan Dela Cruz"));

    [Fact]
    public void Validate_ValidCommand_ReturnsNoErrors()
    {
        var result = _sut.Validate(CreateValidCommand());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyFirstName_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            PersonalInfo = CreateValidCommand().PersonalInfo with { FirstName = "" }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PersonalInfo.FirstName");
    }

    [Fact]
    public void Validate_EmptyLastName_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            PersonalInfo = CreateValidCommand().PersonalInfo with { LastName = "" }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PersonalInfo.LastName");
    }

    [Fact]
    public void Validate_InvalidEmail_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            ContactInfo = CreateValidCommand().ContactInfo with { EmailAddress = "not-an-email" }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ContactInfo.EmailAddress");
    }

    [Fact]
    public void Validate_ConsentNotGiven_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            Consent = CreateValidCommand().Consent with { ConsentGiven = false }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Consent.ConsentGiven");
    }

    [Fact]
    public void Validate_AttestationNotGiven_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            Consent = CreateValidCommand().Consent with { Attestation = false }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Consent.Attestation");
    }

    [Fact]
    public void Validate_MarriedWithoutSpouse_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            PersonalInfo = CreateValidCommand().PersonalInfo with { CivilStatus = "Married" },
            RelatedPersons = new(null, null, null)
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RelatedPersons.Spouse");
    }

    [Fact]
    public void Validate_ExpiryBeforeIssue_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            PrimaryId = CreateValidCommand().PrimaryId with
            {
                IssueDate = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ExpiryDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PrimaryId.ExpiryDate");
    }

    [Fact]
    public void Validate_EmptyTin_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            GovernmentIds = CreateValidCommand().GovernmentIds with { Tin = "" }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "GovernmentIds.Tin");
    }

    [Fact]
    public void Validate_EmptySss_ReturnsError()
    {
        var cmd = CreateValidCommand() with
        {
            GovernmentIds = CreateValidCommand().GovernmentIds with { Sss = "" }
        };

        var result = _sut.Validate(cmd);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "GovernmentIds.Sss");
    }
}
