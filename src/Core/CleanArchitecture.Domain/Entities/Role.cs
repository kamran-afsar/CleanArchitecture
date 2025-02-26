
namespace CleanArchitecture.Domain.Entities;

public class Role : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ICollection<UserRole> UserRoles { get; private set; }

    private Role() { } // For EF Core

    public Role(string name, string description = null)
    {
        Name = name;
        Description = description;
        UserRoles = new List<UserRole>();
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }
} 