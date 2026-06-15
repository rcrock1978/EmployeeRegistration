using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class GovernmentIdsTests
{
    [Fact]
    public void Create_SetsTinAndSss()
    {
        var ids = new GovernmentIds("123-456-789-000", "01-2345678-9");

        Assert.Equal("123-456-789-000", ids.Tin);
        Assert.Equal("01-2345678-9", ids.Sss);
    }
}
