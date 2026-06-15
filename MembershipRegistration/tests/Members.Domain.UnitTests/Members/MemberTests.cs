using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class MemberTests
{
    private static readonly PersonName ValidName = new("Mr.", "Juan", null, "Dela Cruz", null, null);
    private static readonly Demographics ValidDemographics = new(
        new DateTime(1990, 5, 12, 0, 0, 0, DateTimeKind.Utc),
        "Manila", "Philippines", "Filipino", "Male", "Single", null, "College Graduate");
    private static readonly ContactDetails ValidContact = new("juan@email.com", "+639170000000");
    private static readonly DependentInfo ValidDependents = new(0);
    private static readonly RelatedPersons ValidRelatives = new(null, null, null);
    private static readonly GovernmentIds ValidGovIds = new("123-456-789-000", "01-2345678-9");
    private static readonly PrimaryIdentification ValidPrimaryId = new(
        "Passport", "P1234567A",
        new DateTime(2020, 1, 10, 0, 0, 0, DateTimeKind.Utc),
        new DateTime(2030, 1, 9, 0, 0, 0, DateTimeKind.Utc),
        "Philippines");
    private static readonly Address ValidAddress = new(
        "123 Rizal St.", "Manila", "1000", "Poblacion", null,
        "Metro Manila", "Philippines", "Owner",
        new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    private static readonly PermanentAddressInfo ValidPermanent = new(true, null);
    private static readonly EmergencyContact ValidEmergency = new("Maria Santos", "Spouse", "+639170000001");
    private static readonly EmploymentDetails ValidEmployment = new(
        "RNF", "OPTODEV Inc.", "EMP-001", 45000m, "Monthly", "Technician",
        new DateTime(2019, 6, 1, 0, 0, 0, DateTimeKind.Utc), null);
    private static readonly Consent ValidConsent = new(true, true, "Juan Dela Cruz", DateTime.UtcNow);

    [Fact]
    public void Create_WithValidData_SetsAllProperties()
    {
        var member = Member.Create(
            ValidName, ValidDemographics, ValidContact, ValidDependents,
            ValidRelatives, ValidGovIds, ValidPrimaryId,
            ValidAddress, ValidPermanent, ValidEmergency,
            ValidEmployment, ValidConsent);

        Assert.NotEqual(Guid.Empty, member.Id);
        Assert.Equal(MemberStatus.Submitted, member.Status);
        Assert.Equal("juan@email.com", member.EmailAddress);
        Assert.Equal(ValidName, member.PersonName);
        Assert.Equal(ValidContact.EmailAddress, member.EmailAddress);
    }

    [Fact]
    public void UpdateStatus_ChangesStatus()
    {
        var member = Member.Create(
            ValidName, ValidDemographics, ValidContact, ValidDependents,
            ValidRelatives, ValidGovIds, ValidPrimaryId,
            ValidAddress, ValidPermanent, ValidEmergency,
            ValidEmployment, ValidConsent);

        member.UpdateStatus(MemberStatus.Approved);

        Assert.Equal(MemberStatus.Approved, member.Status);
    }

    [Fact]
    public void Create_GeneratesNewGuidEachTime()
    {
        var member1 = CreateDefaultMember();
        var member2 = CreateDefaultMember();

        Assert.NotEqual(member1.Id, member2.Id);
    }

    private static Member CreateDefaultMember() => Member.Create(
        ValidName, ValidDemographics, ValidContact, ValidDependents,
        ValidRelatives, ValidGovIds, ValidPrimaryId,
        ValidAddress, ValidPermanent, ValidEmergency,
        ValidEmployment, ValidConsent);
}
