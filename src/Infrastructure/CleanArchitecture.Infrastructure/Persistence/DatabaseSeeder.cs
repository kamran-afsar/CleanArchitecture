using CleanArchitecture.Application.Interfaces.Utility;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Infrastructure.Services
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            ILogger<DatabaseSeeder>? logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedRolesAsync();
                // Save roles first so that they can be queried when seeding users.
                await _context.SaveChangesAsync();
                await SeedDefaultUsersAsync();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Seeding roles...");

            // Roles.All is assumed to be a Dictionary<string, string> with role names and descriptions.
            foreach (var role in Roles.All)
            {
                if (!await _context.Roles.AnyAsync(r => r.Name == role.Key))
                {
                    _logger.LogInformation("Adding role: {RoleName}", role.Key);
                    await _context.Roles.AddAsync(new Role(role.Key, role.Value));
                }
            }
        }

        private async Task SeedDefaultUsersAsync()
        {
            _logger.LogInformation("Seeding default users...");

            // Define a system user Guid for audit fields (this can be any Guid representing the system)
            var systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

            // Add default admin user if not already present.
            if (!await _context.Users.AnyAsync(u => u.Email == "admin@example.com"))
            {
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.Admin);
                if (adminRole == null)
                {
                    throw new Exception("Admin role not found after seeding roles.");
                }

                var adminUser = new User(
                    "Admin",
                    "User",
                    "admin@example.com",
                    Guid.NewGuid().ToString(),
                    _passwordHasher.HashPassword("Admin123!")

                )
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = systemUserId,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = systemUserId
                };

                adminUser.AddRole(adminRole);
                await _context.Users.AddAsync(adminUser);
                _logger.LogInformation("Added default admin user");
            }

            // Add default regular user if not already present.
            if (!await _context.Users.AnyAsync(u => u.Email == "user@example.com"))
            {
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.User);
                if (userRole == null)
                {
                    throw new Exception("User role not found after seeding roles.");
                }

                var defaultUser = new User(
                    "Default",
                    "User",
                    "user@example.com",
                    Guid.NewGuid().ToString(),
                    _passwordHasher.HashPassword("User123!")
                )
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = systemUserId,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = systemUserId
                };

                defaultUser.AddRole(userRole);
                await _context.Users.AddAsync(defaultUser);
                _logger.LogInformation("Added default regular user");
            }
        }
    }
}
