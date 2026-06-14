using Members.Application.Common.Messaging;
using Members.Domain.Members;

namespace Members.Application.Features.Members.ListMembers;

public sealed record ListMembersQuery(
    string? LastName,
    string? Email,
    string? EmployeeLevel,
    DateTime? CreatedDateFrom,
    DateTime? CreatedDateTo,
    int Page = 1,
    int PageSize = 20
) : IQuery<ListMembersResponse>;

public sealed record ListMembersResponse(
    IReadOnlyList<MemberListItem> Items,
    int TotalCount,
    int Page,
    int PageSize
);
