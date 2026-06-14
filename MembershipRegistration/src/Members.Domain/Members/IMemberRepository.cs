namespace Members.Domain.Members;

public sealed record MemberListItem(Guid Id, string FirstName, string LastName, string Email, string Status, DateTime CreatedOn);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public interface IMemberRepository
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<MemberListItem>> ListAsync(
        string? lastName, string? email, string? employeeLevel,
        DateTime? createdDateFrom, DateTime? createdDateTo,
        int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Member member, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
