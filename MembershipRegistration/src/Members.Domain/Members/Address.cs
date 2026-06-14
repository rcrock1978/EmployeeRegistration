namespace Members.Domain.Members;

public record Address
{
    public string StreetNameAndNumber { get; init; } = null!;
    public string City { get; init; } = null!;
    public string PostalCode { get; init; } = null!;
    public string Barangay { get; init; } = null!;
    public string? SubdivisionPurok { get; init; }
    public string Province { get; init; } = null!;
    public string Country { get; init; } = null!;
    public string OwnerOrLessee { get; init; } = null!;
    public DateTime OccupiedSince { get; init; }

    private Address() { }

    public Address(string streetNameAndNumber, string city, string postalCode, string barangay,
        string? subdivisionPurok, string province, string country, string ownerOrLessee, DateTime occupiedSince)
    {
        StreetNameAndNumber = streetNameAndNumber;
        City = city;
        PostalCode = postalCode;
        Barangay = barangay;
        SubdivisionPurok = subdivisionPurok;
        Province = province;
        Country = country;
        OwnerOrLessee = ownerOrLessee;
        OccupiedSince = occupiedSince;
    }
}

public record PermanentAddressInfo
{
    public bool SameAsCurrent { get; init; }
    public Address? Address { get; init; }

    private PermanentAddressInfo() { }

    public PermanentAddressInfo(bool sameAsCurrent, Address? address)
    {
        SameAsCurrent = sameAsCurrent;
        Address = address;
    }
}
