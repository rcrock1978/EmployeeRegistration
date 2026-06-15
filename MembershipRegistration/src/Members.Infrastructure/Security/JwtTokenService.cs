using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Members.Application.Common;
using Microsoft.IdentityModel.Tokens;

namespace Members.Infrastructure.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly string _signingKey;

    public JwtTokenService(string signingKey)
    {
        _signingKey = signingKey;
    }

    public string GenerateToken(string email, string role)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, email),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
        };

        var token = new JwtSecurityToken(
            issuer: "dev",
            audience: "optodev-members",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
