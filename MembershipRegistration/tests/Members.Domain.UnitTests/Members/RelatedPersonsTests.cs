using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class RelatedPersonsTests
{
    [Fact]
    public void Create_WithAllRelations_SetsProperties()
    {
        var spouse = new SpouseInfo("Maria", "Reyes", "Dela Cruz");
        var father = new FatherInfo("Pedro", "Santos", "Dela Cruz", "Sr.");
        var related = new RelatedPersons(spouse, "Ana Bautista Reyes", father);

        Assert.NotNull(related.Spouse);
        Assert.Equal("Maria", related.Spouse.FirstName);
        Assert.Equal("Reyes", related.Spouse.MiddleName);
        Assert.Equal("Dela Cruz", related.Spouse.LastName);

        Assert.Equal("Ana Bautista Reyes", related.MotherMaidenName);

        Assert.NotNull(related.Father);
        Assert.Equal("Pedro", related.Father.FirstName);
        Assert.Equal("Santos", related.Father.MiddleName);
        Assert.Equal("Dela Cruz", related.Father.LastName);
        Assert.Equal("Sr.", related.Father.Suffix);
    }

    [Fact]
    public void Create_WithNullRelations_SetsNull()
    {
        var related = new RelatedPersons(null, null, null);

        Assert.Null(related.Spouse);
        Assert.Null(related.MotherMaidenName);
        Assert.Null(related.Father);
    }
}
