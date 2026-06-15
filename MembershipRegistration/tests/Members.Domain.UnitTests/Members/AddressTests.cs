using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class AddressTests
{
    private static readonly DateTime OccupiedSince = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithAllFields_SetsProperties()
    {
        var addr = new Address("123 Mabini St.", "Manila", "1000", "San Roque",
            "Purok 2", "Metro Manila", "Philippines", "Owner", OccupiedSince);

        Assert.Equal("123 Mabini St.", addr.StreetNameAndNumber);
        Assert.Equal("Manila", addr.City);
        Assert.Equal("1000", addr.PostalCode);
        Assert.Equal("San Roque", addr.Barangay);
        Assert.Equal("Purok 2", addr.SubdivisionPurok);
        Assert.Equal("Metro Manila", addr.Province);
        Assert.Equal("Philippines", addr.Country);
        Assert.Equal("Owner", addr.OwnerOrLessee);
        Assert.Equal(OccupiedSince, addr.OccupiedSince);
    }

    [Fact]
    public void Create_WithNullSubdivision_SetsNull()
    {
        var addr = new Address("456 Rizal Ave.", "Quezon City", "1100", "Poblacion",
            null, "Metro Manila", "Philippines", "Lessee", OccupiedSince);

        Assert.Null(addr.SubdivisionPurok);
    }
}

public sealed class PermanentAddressInfoTests
{
    [Fact]
    public void SameAsCurrent_True_AddressIsNull()
    {
        var info = new PermanentAddressInfo(true, null);

        Assert.True(info.SameAsCurrent);
        Assert.Null(info.Address);
    }

    [Fact]
    public void SameAsCurrent_False_AddressIsNotNull()
    {
        var addr = new Address("123 Mabini St.", "Manila", "1000", "San Roque",
            null, "Metro Manila", "Philippines", "Owner", DateTime.UtcNow);
        var info = new PermanentAddressInfo(false, addr);

        Assert.False(info.SameAsCurrent);
        Assert.NotNull(info.Address);
    }
}
