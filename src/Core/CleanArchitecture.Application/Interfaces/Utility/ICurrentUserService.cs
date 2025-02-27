namespace CleanArchitecture.Application.Interfaces.Utility
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
        IEnumerable<string> Roles { get; }
    }
}
