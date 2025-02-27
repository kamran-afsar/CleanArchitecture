namespace CleanArchitecture.Application.Interfaces
{
    public interface IUserDapperRepository
    {
        Task<UserDetailDto> GetUserDetailsByIdAsync(Guid userId);
        Task<IEnumerable<UserWithRolesDto>> GetUsersWithRolesAsync();
        Task<int> UpdateUserLastLoginAsync(Guid userId);
    }
}
