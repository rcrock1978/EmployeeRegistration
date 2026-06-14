using FluentValidation;

namespace Members.Application.Features.Members.GetMemberById;

public sealed class GetMemberByIdQueryValidator : AbstractValidator<GetMemberByIdQuery>
{
    public GetMemberByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
