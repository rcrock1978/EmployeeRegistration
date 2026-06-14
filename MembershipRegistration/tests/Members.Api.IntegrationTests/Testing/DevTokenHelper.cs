using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Members.Api.IntegrationTests.Testing;

public static class DevTokenHelper
{
    private const string DevSigningKey = "ThisIsADevelopmentSigningKeyThatIsAtLeast32Bytes!";

    public static string GenerateToken(string subject, string role, string? email = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DevSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, subject),
            new(ClaimTypes.Role, role),
        };

        if (email is not null)
            claims.Add(new(ClaimTypes.Email, email));

        var token = new JwtSecurityToken(
            issuer: "dev",
            audience: "optodev-members",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
