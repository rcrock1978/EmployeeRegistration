using Members.Application.Common.Messaging;
using Members.Application.Common.Results;

namespace Members.Application.Features.Members.UpdateMember;

public sealed record UpdateMemberCommand(
    Guid Id,
    PersonalInfoDto PersonalInfo,
    ContactInfoDto ContactInfo,
    RelatedPersonsDto RelatedPersons,
    GovernmentIdsDto GovernmentIds,
    PrimaryIdDto PrimaryId,
    AddressDto CurrentAddress,
    PermanentAddressDto PermanentAddress,
    EmergencyContactDto EmergencyContact,
    EmploymentDto Employment,
    ConsentDto Consent
) : ICommand<UpdateMemberResponse>;

public sealed record UpdateMemberResponse(Guid Id);

public sealed record PersonalInfoDto(
    string Title, string FirstName, string? MiddleName, string LastName,
    string? Suffix, string? Alias, DateTime DateOfBirth, string PlaceOfBirth,
    string CountryOfBirth, string Nationality, string Gender, string CivilStatus,
    string? Religion, string HighestEducationalAttainment, int NumberOfDependents);

public sealed record ContactInfoDto(string EmailAddress, string ContactNumber);

public sealed record RelatedPersonsDto(SpouseDto? Spouse, string? MotherMaidenName, FatherDto? Father);
public sealed record SpouseDto(string FirstName, string? MiddleName, string LastName);
public sealed record FatherDto(string FirstName, string? MiddleName, string LastName, string? Suffix);

public sealed record GovernmentIdsDto(string Tin, string Sss);
public sealed record PrimaryIdDto(string Type, string Number, DateTime IssueDate, DateTime ExpiryDate, string IssueCountry);

public sealed record AddressDto(
    string StreetNameAndNumber, string City, string PostalCode, string Barangay,
    string? SubdivisionPurok, string Province, string Country,
    string OwnerOrLessee, DateTime OccupiedSince);

public sealed record PermanentAddressDto(bool SameAsCurrent, AddressDto? Address);

public sealed record EmergencyContactDto(string ContactName, string Relationship, string ContactNumber);

public sealed record EmploymentDto(
    string EmployeeLevel, string CompanyTradeName, string CompanyIdNumber,
    decimal GrossIncome, string IncomePeriod, string Occupation,
    DateTime HiredFrom, DateTime? HiredTo);

public sealed record ConsentDto(bool ConsentGiven, bool Attestation, string SignatureName);
