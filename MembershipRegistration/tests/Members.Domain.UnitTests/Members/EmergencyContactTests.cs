using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class EmergencyContactTests
{
    [Fact]
    public void Create_SetsAllProperties()
    {
        var contact = new EmergencyContact("Maria Dela Cruz", "Spouse", "+639170000001");

        Assert.Equal("Maria Dela Cruz", contact.ContactName);
        Assert.Equal("Spouse", contact.Relationship);
        Assert.Equal("+639170000001", contact.ContactNumber);
    }
}
