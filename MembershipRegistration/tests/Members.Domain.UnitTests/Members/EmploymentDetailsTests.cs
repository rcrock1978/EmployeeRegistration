using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class EmploymentDetailsTests
{
    private static readonly DateTime HiredFrom = new(2019, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithAllFields_SetsProperties()
    {
        var employment = new EmploymentDetails("RNF", "OPTODEV Inc.", "EMP-001",
            45000m, "Monthly", "Technician", HiredFrom, null);

        Assert.Equal("RNF", employment.EmployeeLevel);
        Assert.Equal("OPTODEV Inc.", employment.CompanyTradeName);
        Assert.Equal("EMP-001", employment.CompanyIdNumber);
        Assert.Equal(45000m, employment.GrossIncome);
        Assert.Equal("Monthly", employment.IncomePeriod);
        Assert.Equal("Technician", employment.Occupation);
        Assert.Equal(HiredFrom, employment.HiredFrom);
        Assert.Null(employment.HiredTo);
    }

    [Fact]
    public void Create_WithHiredTo_SetsProperty()
    {
        var hiredTo = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var employment = new EmploymentDetails("Manager", "Acme Corp", "EMP-002",
            85000m, "Monthly", "Manager", HiredFrom, hiredTo);

        Assert.Equal(hiredTo, employment.HiredTo);
    }
}
