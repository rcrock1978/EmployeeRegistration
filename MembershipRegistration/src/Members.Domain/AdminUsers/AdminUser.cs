namespace Members.Domain.AdminUsers;

public sealed class AdminUser
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; }
    public DateTime CreatedOn { get; private set; }

    private AdminUser()
    {
        Email = null!;
        PasswordHash = null!;
        Role = null!;
    }

    public AdminUser(string email, string passwordHash, string role)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedOn = DateTime.UtcNow;
    }

    public void UpdateRole(string newRole)
    {
        Role = newRole;
    }
}
