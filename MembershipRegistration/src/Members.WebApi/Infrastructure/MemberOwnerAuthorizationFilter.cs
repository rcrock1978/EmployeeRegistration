using System.Security.Claims;
using Members.Application.Common;
using Members.Domain.Members;

namespace Members.WebApi.Infrastructure;

public sealed class MemberOwnerAuthorizationFilter
{
    private readonly ICurrentUserService _currentUser;
    private readonly IMemberRepository _repository;
    private readonly ILogger<MemberOwnerAuthorizationFilter> _logger;

    public MemberOwnerAuthorizationFilter(
        ICurrentUserService currentUser,
        IMemberRepository repository,
        ILogger<MemberOwnerAuthorizationFilter> logger)
    {
        _currentUser = currentUser;
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> IsOwnerOrAdminAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        if (_currentUser.IsInRole("HRAdmin") || _currentUser.IsInRole("Admin"))
            return true;

        var sub = _currentUser.UserId;
        if (string.IsNullOrWhiteSpace(sub))
            return false;

        var member = await _repository.GetByIdAsync(memberId, cancellationToken);
        if (member is null)
            return false;

        var isOwner = string.Equals(member.EmailAddress, sub, StringComparison.OrdinalIgnoreCase);
        if (!isOwner)
        {
            _logger.LogWarning(
                "Unauthorized access attempt: User {Sub} tried to access Member {MemberId}", sub, memberId);
        }

        return isOwner;
    }
}
