using FluentValidation;

namespace Members.Application.Features.Members.RegisterMember;

public sealed class RegisterMemberCommandValidator : AbstractValidator<RegisterMemberCommand>
{
    public RegisterMemberCommandValidator()
    {
        RuleFor(x => x.PersonalInfo.FirstName).NotEmpty().Length(2, 100);
        RuleFor(x => x.PersonalInfo.LastName).NotEmpty().Length(2, 100);
        RuleFor(x => x.PersonalInfo.MiddleName).MaximumLength(100);
        RuleFor(x => x.PersonalInfo.Alias).MaximumLength(100);
        RuleFor(x => x.PersonalInfo.DateOfBirth).NotEmpty();
        RuleFor(x => x.PersonalInfo.PlaceOfBirth).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PersonalInfo.CountryOfBirth).NotEmpty();
        RuleFor(x => x.PersonalInfo.Nationality).NotEmpty();
        RuleFor(x => x.PersonalInfo.Gender).NotEmpty();
        RuleFor(x => x.PersonalInfo.CivilStatus).NotEmpty();
        RuleFor(x => x.PersonalInfo.HighestEducationalAttainment).NotEmpty();
        RuleFor(x => x.PersonalInfo.NumberOfDependents).GreaterThanOrEqualTo(0);

        RuleFor(x => x.ContactInfo.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(x => x.ContactInfo.ContactNumber).NotEmpty();

        RuleFor(x => x.GovernmentIds.Tin).NotEmpty();
        RuleFor(x => x.GovernmentIds.Sss).NotEmpty();

        RuleFor(x => x.PrimaryId.Type).NotEmpty();
        RuleFor(x => x.PrimaryId.Number).NotEmpty();
        RuleFor(x => x.PrimaryId.IssueDate).NotEmpty();
        RuleFor(x => x.PrimaryId.ExpiryDate).NotEmpty();
        RuleFor(x => x.PrimaryId.IssueCountry).NotEmpty();

        RuleFor(x => x.CurrentAddress.StreetNameAndNumber).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CurrentAddress.City).NotEmpty();
        RuleFor(x => x.CurrentAddress.PostalCode).NotEmpty();
        RuleFor(x => x.CurrentAddress.Barangay).NotEmpty();
        RuleFor(x => x.CurrentAddress.Province).NotEmpty();
        RuleFor(x => x.CurrentAddress.Country).NotEmpty();
        RuleFor(x => x.CurrentAddress.OwnerOrLessee).NotEmpty();
        RuleFor(x => x.CurrentAddress.OccupiedSince).NotEmpty();

        RuleFor(x => x.EmergencyContact.ContactName).NotEmpty();
        RuleFor(x => x.EmergencyContact.Relationship).NotEmpty();
        RuleFor(x => x.EmergencyContact.ContactNumber).NotEmpty();

        RuleFor(x => x.Employment.EmployeeLevel).NotEmpty();
        RuleFor(x => x.Employment.CompanyTradeName).NotEmpty();
        RuleFor(x => x.Employment.CompanyIdNumber).NotEmpty();
        RuleFor(x => x.Employment.GrossIncome).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Employment.IncomePeriod).NotEmpty();
        RuleFor(x => x.Employment.Occupation).NotEmpty();
        RuleFor(x => x.Employment.HiredFrom).NotEmpty();

        RuleFor(x => x.Consent.ConsentGiven).Equal(true);
        RuleFor(x => x.Consent.Attestation).Equal(true);
        RuleFor(x => x.Consent.SignatureName).NotEmpty();

        RuleFor(x => x.RelatedPersons.Spouse)
            .NotNull()
            .When(x => x.PersonalInfo.CivilStatus == "Married");

        RuleFor(x => x.PrimaryId.ExpiryDate)
            .GreaterThan(x => x.PrimaryId.IssueDate);
    }
}
