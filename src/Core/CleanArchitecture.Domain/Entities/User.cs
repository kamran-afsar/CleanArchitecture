
namespace CleanArchitecture.Domain.Entities;

public class User : EntityBase
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public DateTime? LastLoginDate { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<UserRole> UserRoles { get; private set; }

    private User() { } // For EF Core

    public User(string firstName, string lastName, string email, string passwordHash, string refreshToken)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
        RefreshToken = refreshToken;
        UserRoles = new List<UserRole>();
    }

    public void UpdateRefreshToken(string token, DateTime expiryTime)
    {
        RefreshToken = token;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void IncrementFailedAttempts()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            LockoutEnd = DateTime.UtcNow.AddMinutes(15);
        }
    }

    public void AddRole(Role role)
    {
        if (role != null)
        {
            if (UserRoles.Any(ur => ur.RoleId == role.Id))
                return;

            UserRoles.Add(new UserRole(this.Id, role.Id));
        }

    }

    public void RemoveRole(Role role)
    {
        var userRole = UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole != null)
        {
            UserRoles.Remove(userRole);
        }
    }
    public void ResetFailedAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEnd = null;
    }

    public void UpdateLastLogin()
    {
        LastLoginDate = DateTime.UtcNow;
    }

    public void Update(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}