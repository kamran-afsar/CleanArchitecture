using Microsoft.Extensions.Logging;

public interface IUserDapperRepository
{
    Task<UserDetailDto> GetUserDetailsByIdAsync(Guid userId);
    Task<IEnumerable<UserWithRolesDto>> GetUsersWithRolesAsync();
    Task<int> UpdateUserLastLoginAsync(Guid userId);
}

public class UserDapperRepository : DapperRepositoryBase, IUserDapperRepository
{
    public UserDapperRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<UserDapperRepository> logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<UserDetailDto> GetUserDetailsByIdAsync(Guid userId)
    {
        const string storedProcedure = "sp_GetUserDetails";
        var parameters = new { UserId = userId };

        return await ExecuteStoredProcedureAsync<UserDetailDto>(
            storedProcedure,
            parameters);
    }

    public async Task<IEnumerable<UserWithRolesDto>> GetUsersWithRolesAsync()
    {
        const string storedProcedure = "sp_GetUsersWithRoles";

        return await ExecuteStoredProcedureListAsync<UserWithRolesDto>(
            storedProcedure);
    }

    public async Task<int> UpdateUserLastLoginAsync(Guid userId)
    {
        const string storedProcedure = "sp_UpdateUserLastLogin";
        var parameters = new
        {
            UserId = userId,
            LastLoginDate = DateTime.UtcNow
        };

        return await ExecuteStoredProcedureNonQueryAsync(
            storedProcedure,
            parameters);
    }
}