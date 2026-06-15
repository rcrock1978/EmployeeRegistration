using Members.Domain.Members;

namespace Members.Domain.UnitTests.Members;

public sealed class DependentInfoTests
{
    [Fact]
    public void Create_WithDependents_SetsNumber()
    {
        var info = new DependentInfo(3);

        Assert.Equal(3, info.NumberOfDependents);
    }

    [Fact]
    public void Create_WithZeroDependents_SetsZero()
    {
        var info = new DependentInfo(0);

        Assert.Equal(0, info.NumberOfDependents);
    }
}
