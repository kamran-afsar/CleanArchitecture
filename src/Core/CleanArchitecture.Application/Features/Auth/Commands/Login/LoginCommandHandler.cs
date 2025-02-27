using AutoMapper;
using CleanArchitecture.Application.Interfaces;

using CleanArchitecture.Application.Interfaces.Utility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<LoginCommandHandler> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync();


            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
                throw new UnauthorizedException("Invalid credentials");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for user {Email}", request.Email);
                throw new UnauthorizedException("Invalid credentials");
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                _logger.LogWarning("Login failed: User {Email} is locked out until {LockoutEnd}",
                    request.Email, user.LockoutEnd);
                throw new UnauthorizedException("Account is locked. Please try again later.");
            }

            // Reset failed attempts on successful login
            user.ResetFailedAttempts();
            user.UpdateLastLogin();

            // Generate tokens
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var token = _jwtService.GenerateJwtToken(user.Id, user.Email, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token
            user.UpdateRefreshToken(refreshToken.Token, refreshToken.ExpiresAt);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Email} logged in successfully", request.Email);

            return new LoginResponse
            {
                Token = token.Token,
                RefreshToken = refreshToken.Token,
                ExpiresAt = token.ExpiresAt,
                User = _mapper.Map<UserDto>(user)
            };
        }
    }
}