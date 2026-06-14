using Members.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace Members.Infrastructure.Persistence;

public sealed class MemberRepository : IMemberRepository
{
    private readonly MembersDbContext _context;

    public MemberRepository(MembersDbContext context)
    {
        _context = context;
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

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
