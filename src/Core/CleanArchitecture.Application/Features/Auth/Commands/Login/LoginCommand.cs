using MediatR;

public record LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; init; }
    public string Password { get; init; }
}

public class LoginResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; }
}