using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.Utility;
using CleanArchitecture.Domain.Entities;
using MediatR;

namespace CleanArchitecture.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;

        public CreateUserCommandHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(
                request.FirstName,
                request.LastName,
                request.Email,
                _passwordHasher.HashPassword(request.Password)
            );

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Send welcome email asynchronously
            await SendWelcomeEmailAsync(user);

            return user.Id;
        }

        private async Task SendWelcomeEmailAsync(User user)
        {
            var emailBody = EmailTemplateService.GetWelcomeEmailTemplate(user.FirstName);
            await _emailService.SendEmailAsync(
                user.Email,
                "Welcome to Our Platform",
                emailBody
            );
        }
    }
}
