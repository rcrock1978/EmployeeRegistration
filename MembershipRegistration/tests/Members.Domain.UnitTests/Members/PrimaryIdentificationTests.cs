using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class PrimaryIdentificationTests
{
    private static readonly DateTime IssueDate = new(2020, 1, 10, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime ExpiryDate = new(2030, 1, 9, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_SetsAllProperties()
    {
        var id = new PrimaryIdentification("Passport", "P1234567A", IssueDate, ExpiryDate, "Philippines");

        Assert.Equal("Passport", id.Type);
        Assert.Equal("P1234567A", id.Number);
        Assert.Equal(IssueDate, id.IssueDate);
        Assert.Equal(ExpiryDate, id.ExpiryDate);
        Assert.Equal("Philippines", id.IssueCountry);
    }
}
