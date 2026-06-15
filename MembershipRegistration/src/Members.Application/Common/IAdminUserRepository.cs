using Members.Domain.AdminUsers;

namespace Members.Application.Common;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(Guid id, string newRole, CancellationToken cancellationToken = default);
}
