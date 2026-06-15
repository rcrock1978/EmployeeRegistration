using Members.Application.Features.Members.ListMembers;

namespace Members.Application.UnitTests.Features.Members.ListMembers;

public sealed class ListMembersQueryValidatorTests
{
    private readonly ListMembersQueryValidator _sut = new();

    [Fact]
    public void Validate_DefaultQuery_ReturnsNoErrors()
    {
        var query = new ListMembersQuery(null, null, null, null, null);

        var result = _sut.Validate(query);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithLastName_ReturnsNoErrors()
    {
        var query = new ListMembersQuery("Dela Cruz", null, null, null, null);

        var result = _sut.Validate(query);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ExcessivePageSize_ReturnsError()
    {
        var query = new ListMembersQuery(null, null, null, null, null, 1, 200);

        var result = _sut.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_ZeroPage_ReturnsError()
    {
        var query = new ListMembersQuery(null, null, null, null, null, 0, 20);

        var result = _sut.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Page");
    }
}
