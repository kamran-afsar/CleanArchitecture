using CleanArchitecture.Domain.Entities;
using System;

public class UserRole : EntityBase
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; }

    private UserRole() { } // For EF Core

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
} 