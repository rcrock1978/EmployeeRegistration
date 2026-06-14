using Members.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Members.Infrastructure.Persistence;

public sealed class MemberRepository : IMemberRepository
{
    private static readonly Action<ILogger, Guid, Exception?> LogSensitiveDataAccess =
        LoggerMessage.Define<Guid>(LogLevel.Information, new EventId(1, "SensitiveDataAccessed"),
            "Sensitive data accessed: Member {MemberId}");

    private readonly MembersDbContext _context;
    private readonly ILogger<MemberRepository> _logger;

    public MemberRepository(MembersDbContext context, ILogger<MemberRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Set<Member>().AnyAsync(m => m.EmailAddress == email, cancellationToken);
    }

    public Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        _context.Set<Member>().Add(member);
        return Task.CompletedTask;
    }

    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var member = await _context.Set<Member>().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (member is not null)
        {
            LogSensitiveDataAccess(_logger, id, null);
        }

        return member;
    }

    public async Task<PagedResult<MemberListItem>> ListAsync(
        string? lastName, string? email, string? employeeLevel,
        DateTime? createdDateFrom, DateTime? createdDateTo,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Member>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(lastName))
            query = query.Where(m => m.PersonName.LastName.Contains(lastName));

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(m => m.EmailAddress.Contains(email));

        if (!string.IsNullOrWhiteSpace(employeeLevel))
            query = query.Where(m => m.EmploymentDetails.EmployeeLevel == employeeLevel);

        if (createdDateFrom.HasValue)
            query = query.Where(m => m.CreatedOn >= createdDateFrom.Value);

        if (createdDateTo.HasValue)
            query = query.Where(m => m.CreatedOn <= createdDateTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MemberListItem(
                m.Id,
                m.PersonName.FirstName,
                m.PersonName.LastName,
                m.EmailAddress,
                m.Status.ToString(),
                m.CreatedOn))
            .ToListAsync(cancellationToken);

        return new PagedResult<MemberListItem>(items, totalCount, page, pageSize);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
