using Members.Application.Common;
using Members.Application.Common.Messaging;
using Members.Application.Common.Results;
using Members.Domain.Members;

namespace Members.Application.Features.Members.RegisterMember;

public sealed class RegisterMemberCommandHandler : ICommandHandler<RegisterMemberCommand, RegisterMemberResponse>
{
    private readonly IMemberRepository _repository;
    private readonly IMemberSubmissionLogger _submissionLogger;

    public RegisterMemberCommandHandler(IMemberRepository repository, IMemberSubmissionLogger submissionLogger)
    {
        _repository = repository;
        _submissionLogger = submissionLogger;
    }

    public async Task<Result<RegisterMemberResponse>> Handle(RegisterMemberCommand command, CancellationToken cancellationToken)
    {
        var emailExists = await _repository.ExistsByEmailAsync(command.ContactInfo.EmailAddress, cancellationToken);

        if (emailExists)
        {
            return Result.Failure<RegisterMemberResponse>(new AppError(
                "Conflict.DuplicateEmail",
                "A member with this email address already exists.",
                new List<FieldError>
                {
                    new("contactInfo.emailAddress", "Email.Duplicate", "This email is already registered.")
                }));
        }

        var personName = new PersonName(
            command.PersonalInfo.Title,
            command.PersonalInfo.FirstName,
            command.PersonalInfo.MiddleName,
            command.PersonalInfo.LastName,
            command.PersonalInfo.Suffix,
            command.PersonalInfo.Alias);

        var demographics = new Demographics(
            command.PersonalInfo.DateOfBirth,
            command.PersonalInfo.PlaceOfBirth,
            command.PersonalInfo.CountryOfBirth,
            command.PersonalInfo.Nationality,
            command.PersonalInfo.Gender,
            command.PersonalInfo.CivilStatus,
            command.PersonalInfo.Religion,
            command.PersonalInfo.HighestEducationalAttainment);

        var contactDetails = new ContactDetails(
            command.ContactInfo.EmailAddress,
            command.ContactInfo.ContactNumber);

        var dependentInfo = new DependentInfo(command.PersonalInfo.NumberOfDependents);

        var spouse = command.RelatedPersons.Spouse is not null
            ? new SpouseInfo(command.RelatedPersons.Spouse.FirstName, command.RelatedPersons.Spouse.MiddleName, command.RelatedPersons.Spouse.LastName)
            : null;

        var father = command.RelatedPersons.Father is not null
            ? new FatherInfo(command.RelatedPersons.Father.FirstName, command.RelatedPersons.Father.MiddleName, command.RelatedPersons.Father.LastName, command.RelatedPersons.Father.Suffix)
            : null;

        var relatedPersons = new RelatedPersons(spouse, command.RelatedPersons.MotherMaidenName, father);

        var governmentIds = new GovernmentIds(command.GovernmentIds.Tin, command.GovernmentIds.Sss);

        var primaryId = new PrimaryIdentification(
            command.PrimaryId.Type,
            command.PrimaryId.Number,
            command.PrimaryId.IssueDate,
            command.PrimaryId.ExpiryDate,
            command.PrimaryId.IssueCountry);

        var currentAddress = new Address(
            command.CurrentAddress.StreetNameAndNumber,
            command.CurrentAddress.City,
            command.CurrentAddress.PostalCode,
            command.CurrentAddress.Barangay,
            command.CurrentAddress.SubdivisionPurok,
            command.CurrentAddress.Province,
            command.CurrentAddress.Country,
            command.CurrentAddress.OwnerOrLessee,
            command.CurrentAddress.OccupiedSince);

        var permanentAddress = command.PermanentAddress.Address is not null
            ? new PermanentAddressInfo(false, new Address(
                command.PermanentAddress.Address.StreetNameAndNumber,
                command.PermanentAddress.Address.City,
                command.PermanentAddress.Address.PostalCode,
                command.PermanentAddress.Address.Barangay,
                command.PermanentAddress.Address.SubdivisionPurok,
                command.PermanentAddress.Address.Province,
                command.PermanentAddress.Address.Country,
                command.PermanentAddress.Address.OwnerOrLessee,
                command.PermanentAddress.Address.OccupiedSince))
            : new PermanentAddressInfo(true, null);

        var emergencyContact = new EmergencyContact(
            command.EmergencyContact.ContactName,
            command.EmergencyContact.Relationship,
            command.EmergencyContact.ContactNumber);

        var employmentDetails = new EmploymentDetails(
            command.Employment.EmployeeLevel,
            command.Employment.CompanyTradeName,
            command.Employment.CompanyIdNumber,
            command.Employment.GrossIncome,
            command.Employment.IncomePeriod,
            command.Employment.Occupation,
            command.Employment.HiredFrom,
            command.Employment.HiredTo);

        var consent = new Consent(
            command.Consent.ConsentGiven,
            command.Consent.Attestation,
            command.Consent.SignatureName,
            DateTime.UtcNow);

        var member = Domain.Members.Member.Create(
            personName, demographics, contactDetails, dependentInfo, relatedPersons,
            governmentIds, primaryId, currentAddress, permanentAddress,
            emergencyContact, employmentDetails, consent);

        await _repository.AddAsync(member, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _submissionLogger.LogSubmissionAsync(
            member.Id,
            command.PersonalInfo.FirstName,
            command.PersonalInfo.LastName,
            command);

        return Result.Success(new RegisterMemberResponse(member.Id));
    }
}
