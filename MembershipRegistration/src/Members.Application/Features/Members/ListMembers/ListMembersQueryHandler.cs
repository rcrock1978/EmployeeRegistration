using Members.Application.Common.Messaging;
using Members.Application.Common.Results;
using Members.Domain.Members;

namespace Members.Application.Features.Members.ListMembers;

public sealed class ListMembersQueryHandler : IQueryHandler<ListMembersQuery, ListMembersResponse>
{
    private readonly IMemberRepository _repository;

    public ListMembersQueryHandler(IMemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ListMembersResponse>> Handle(ListMembersQuery query, CancellationToken cancellationToken)
    {
        var result = await _repository.ListAsync(
            query.LastName, query.Email, query.EmployeeLevel,
            query.CreatedDateFrom, query.CreatedDateTo,
            Math.Max(1, query.Page), Math.Clamp(query.PageSize, 1, 100),
            cancellationToken);

        var response = new ListMembersResponse(
            result.Items,
            result.TotalCount,
            result.Page,
            result.PageSize);

        return Result.Success(response);
    }
}
