using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class PersonNameTests
{
    [Fact]
    public void Create_WithAllRequiredFields_SetsProperties()
    {
        var name = new PersonName("Mr.", "Juan", "Santos", "Dela Cruz", "Jr.", "John");

        Assert.Equal("Mr.", name.Title);
        Assert.Equal("Juan", name.FirstName);
        Assert.Equal("Santos", name.MiddleName);
        Assert.Equal("Dela Cruz", name.LastName);
        Assert.Equal("Jr.", name.Suffix);
        Assert.Equal("John", name.Alias);
    }

    [Fact]
    public void Create_WithNullOptionals_SetsNull()
    {
        var name = new PersonName("Ms.", "Maria", null, "Santos", null, null);

        Assert.Equal("Ms.", name.Title);
        Assert.Equal("Maria", name.FirstName);
        Assert.Null(name.MiddleName);
        Assert.Equal("Santos", name.LastName);
        Assert.Null(name.Suffix);
        Assert.Null(name.Alias);
    }
}
