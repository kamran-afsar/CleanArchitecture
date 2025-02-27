using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand : IRequest<Unit>
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUserCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.Id);
            }

            user.Update(request.FirstName, request.LastName, request.Email);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}