using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand : IRequest<Guid>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }
}