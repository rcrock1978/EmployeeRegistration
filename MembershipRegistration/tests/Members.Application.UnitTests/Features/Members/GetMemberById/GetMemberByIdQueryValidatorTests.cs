using Members.Application.Features.Members.GetMemberById;

namespace Members.Application.UnitTests.Features.Members.GetMemberById;

public sealed class GetMemberByIdQueryValidatorTests
{
    private readonly GetMemberByIdQueryValidator _sut = new();

    [Fact]
    public void Validate_ValidId_ReturnsNoErrors()
    {
        var query = new GetMemberByIdQuery(Guid.NewGuid());

        var result = _sut.Validate(query);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyId_ReturnsError()
    {
        var query = new GetMemberByIdQuery(Guid.Empty);

        var result = _sut.Validate(query);

        Assert.False(result.IsValid);
    }
}
