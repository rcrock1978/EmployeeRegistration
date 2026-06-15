using Members.Application.Common;
using Members.Domain.AdminUsers;
using Microsoft.EntityFrameworkCore;

namespace Members.Infrastructure.Persistence;

public sealed class AdminUserRepository : IAdminUserRepository
{
    private readonly MembersDbContext _context;

    public AdminUserRepository(MembersDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.AdminUsers.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task UpdateRoleAsync(Guid id, string newRole, CancellationToken cancellationToken = default)
    {
        var admin = await _context.AdminUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (admin is not null)
        {
            admin.UpdateRole(newRole);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
