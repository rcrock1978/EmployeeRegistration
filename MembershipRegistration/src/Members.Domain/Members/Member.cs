using Members.Domain.Common;

namespace Members.Domain.Members;

public class Member : AuditableEntity
{
    public Guid Id { get; private set; }
    public MemberStatus Status { get; private set; }
    public string EmailAddress { get; private set; } = string.Empty;

    public PersonName PersonName { get; private set; } = null!;
    public Demographics Demographics { get; private set; } = null!;
    public ContactDetails ContactDetails { get; private set; } = null!;
    public DependentInfo DependentInfo { get; private set; } = null!;
    public RelatedPersons RelatedPersons { get; private set; } = null!;
    public GovernmentIds GovernmentIds { get; private set; } = null!;
    public PrimaryIdentification PrimaryIdentification { get; private set; } = null!;
    public Address CurrentAddress { get; private set; } = null!;
    public PermanentAddressInfo PermanentAddress { get; private set; } = null!;
    public EmergencyContact EmergencyContact { get; private set; } = null!;
    public EmploymentDetails EmploymentDetails { get; private set; } = null!;
    public Consent Consent { get; private set; } = null!;

    private Member() { }

    public static Member Create(
        PersonName personName,
        Demographics demographics,
        ContactDetails contactDetails,
        DependentInfo dependentInfo,
        RelatedPersons relatedPersons,
        GovernmentIds governmentIds,
        PrimaryIdentification primaryIdentification,
        Address currentAddress,
        PermanentAddressInfo permanentAddress,
        EmergencyContact emergencyContact,
        EmploymentDetails employmentDetails,
        Consent consent)
    {
        return new Member
        {
            Id = Guid.NewGuid(),
            Status = MemberStatus.Submitted,
            PersonName = personName,
            Demographics = demographics,
            ContactDetails = contactDetails,
            EmailAddress = contactDetails.EmailAddress,
            DependentInfo = dependentInfo,
            RelatedPersons = relatedPersons,
            GovernmentIds = governmentIds,
            PrimaryIdentification = primaryIdentification,
            CurrentAddress = currentAddress,
            PermanentAddress = permanentAddress,
            EmergencyContact = emergencyContact,
            EmploymentDetails = employmentDetails,
            Consent = consent
        };
    }

    public void UpdateStatus(MemberStatus newStatus)
    {
        Status = newStatus;
    }

    public void Update(
        PersonName personName,
        Demographics demographics,
        ContactDetails contactDetails,
        DependentInfo dependentInfo,
        RelatedPersons relatedPersons,
        GovernmentIds governmentIds,
        PrimaryIdentification primaryIdentification,
        Address currentAddress,
        PermanentAddressInfo permanentAddress,
        EmergencyContact emergencyContact,
        EmploymentDetails employmentDetails,
        Consent consent)
    {
        PersonName = personName;
        Demographics = demographics;
        ContactDetails = contactDetails;
        EmailAddress = contactDetails.EmailAddress;
        DependentInfo = dependentInfo;
        RelatedPersons = relatedPersons;
        GovernmentIds = governmentIds;
        PrimaryIdentification = primaryIdentification;
        CurrentAddress = currentAddress;
        PermanentAddress = permanentAddress;
        EmergencyContact = emergencyContact;
        EmploymentDetails = employmentDetails;
        Consent = consent;
    }
}
