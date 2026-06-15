using Members.Application.Common;
using Members.Application.Common.Messaging;
using Members.Application.Common.Results;

namespace Members.Application.Features.Auth;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IAdminUserRepository adminUserRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher)
    {
        _adminUserRepository = adminUserRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var admin = await _adminUserRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (admin is null)
        {
            return Result.Failure<LoginResponse>(new AppError(
                "Auth.InvalidCredentials",
                "Invalid email or password."));
        }

        if (!_passwordHasher.Verify(command.Password, admin.PasswordHash))
        {
            return Result.Failure<LoginResponse>(new AppError(
                "Auth.InvalidCredentials",
                "Invalid email or password."));
        }

        var token = _jwtTokenService.GenerateToken(admin.Email, admin.Role);
        return Result<LoginResponse>.Success(new LoginResponse(token, admin.Email, admin.Role));
    }
}
