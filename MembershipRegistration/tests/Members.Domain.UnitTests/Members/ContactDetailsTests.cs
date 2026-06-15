using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class ContactDetailsTests
{
    [Fact]
    public void Create_SetsEmailAndContactNumber()
    {
        var details = new ContactDetails("juan@example.com", "+639170000000");

        Assert.Equal("juan@example.com", details.EmailAddress);
        Assert.Equal("+639170000000", details.ContactNumber);
    }
}
