using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class DemographicsTests
{
    private static readonly DateTime BirthDate = new(1990, 5, 12, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_SetsAllProperties()
    {
        var demo = new Demographics(BirthDate, "Manila", "Philippines", "Filipino",
            "Male", "Married", "Roman Catholic", "Bachelor's Degree");

        Assert.Equal(BirthDate, demo.DateOfBirth);
        Assert.Equal("Manila", demo.PlaceOfBirth);
        Assert.Equal("Philippines", demo.CountryOfBirth);
        Assert.Equal("Filipino", demo.Nationality);
        Assert.Equal("Male", demo.Gender);
        Assert.Equal("Married", demo.CivilStatus);
        Assert.Equal("Roman Catholic", demo.Religion);
        Assert.Equal("Bachelor's Degree", demo.HighestEducationalAttainment);
    }

    [Fact]
    public void Create_WithNullReligion_SetsNull()
    {
        var demo = new Demographics(BirthDate, "Cebu", "Philippines", "Filipino",
            "Female", "Single", null, "High School Graduate");

        Assert.Null(demo.Religion);
    }
}
