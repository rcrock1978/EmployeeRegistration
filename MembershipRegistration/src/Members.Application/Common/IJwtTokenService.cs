namespace Members.Application.Common;

public interface IJwtTokenService
{
    string GenerateToken(string email, string role);
}
