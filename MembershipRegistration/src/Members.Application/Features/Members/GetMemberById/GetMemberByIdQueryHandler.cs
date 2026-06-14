using Members.Application.Common.Messaging;
using Members.Application.Common.Results;
using Members.Domain.Members;

namespace Members.Application.Features.Members.GetMemberById;

public sealed class GetMemberByIdQueryHandler : IQueryHandler<GetMemberByIdQuery, GetMemberByIdResponse>
{
    private readonly IMemberRepository _repository;

    public GetMemberByIdQueryHandler(IMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<GetMemberByIdResponse>> Handle(GetMemberByIdQuery query, CancellationToken cancellationToken)
    {
        var member = await _repository.GetByIdAsync(query.Id, cancellationToken);

        if (member is null)
        {
            return Result.Failure<GetMemberByIdResponse>(new AppError(
                "NotFound", $"Member with ID {query.Id} was not found."));
        }

        var response = MapToResponse(member);
        return Result.Success(response);
    }

    private static GetMemberByIdResponse MapToResponse(Member member)
    {
        var personal = member.PersonName;
        var demo = member.Demographics;
        var ids = member.GovernmentIds;
        var pid = member.PrimaryIdentification;
        var curAddr = member.CurrentAddress;
        var permAddr = member.PermanentAddress;
        var emerg = member.EmergencyContact;
        var emp = member.EmploymentDetails;
        var con = member.Consent;
        var related = member.RelatedPersons;

        return new GetMemberByIdResponse(
            member.Id,
            new MemberStatusDto(member.Status.ToString(), member.Status.ToString()),
            new PersonalInfoDto(
                personal.Title, personal.FirstName, personal.MiddleName, personal.LastName,
                personal.Suffix, personal.Alias, demo.DateOfBirth, demo.PlaceOfBirth,
                demo.CountryOfBirth, demo.Nationality, demo.Gender, demo.CivilStatus,
                demo.Religion, demo.HighestEducationalAttainment, member.DependentInfo.NumberOfDependents),
            new ContactInfoDto(member.ContactDetails.EmailAddress, member.ContactDetails.ContactNumber),
            new RelatedPersonsDto(
                related.Spouse is not null
                    ? new SpouseDto(related.Spouse.FirstName, related.Spouse.MiddleName, related.Spouse.LastName)
                    : null,
                related.MotherMaidenName,
                related.Father is not null
                    ? new FatherDto(related.Father.FirstName, related.Father.MiddleName, related.Father.LastName, related.Father.Suffix)
                    : null),
            new GovernmentIdsDto(ids.Tin, ids.Sss),
            new PrimaryIdDto(pid.Type, pid.Number, pid.IssueDate, pid.ExpiryDate, pid.IssueCountry),
            new AddressDto(
                curAddr.StreetNameAndNumber, curAddr.City, curAddr.PostalCode, curAddr.Barangay,
                curAddr.SubdivisionPurok, curAddr.Province, curAddr.Country,
                curAddr.OwnerOrLessee, curAddr.OccupiedSince),
            permAddr.Address is not null
                ? new PermanentAddressDto(false, new AddressDto(
                    permAddr.Address.StreetNameAndNumber, permAddr.Address.City, permAddr.Address.PostalCode,
                    permAddr.Address.Barangay, permAddr.Address.SubdivisionPurok, permAddr.Address.Province,
                    permAddr.Address.Country, permAddr.Address.OwnerOrLessee, permAddr.Address.OccupiedSince))
                : new PermanentAddressDto(true, null),
            new EmergencyContactDto(emerg.ContactName, emerg.Relationship, emerg.ContactNumber),
            new EmploymentDto(
                emp.EmployeeLevel, emp.CompanyTradeName, emp.CompanyIdNumber,
                emp.GrossIncome, emp.IncomePeriod, emp.Occupation,
                emp.HiredFrom, emp.HiredTo),
            new ConsentDto(con.ConsentGiven, con.Attestation, con.SignatureName)
        );
    }
}
