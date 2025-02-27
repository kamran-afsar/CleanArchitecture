using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserDetails
{
    public record GetUserDetailsQuery(Guid UserId) : IRequest<UserDetailDto>;

    public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, UserDetailDto>
    {
        private readonly IUserDapperRepository _userRepository;
        private readonly ILogger<GetUserDetailsQueryHandler> _logger;

        public GetUserDetailsQueryHandler(
            IUserDapperRepository userRepository,
            ILogger<GetUserDetailsQueryHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserDetailDto> Handle(
            GetUserDetailsQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting user details for user {UserId}", request.UserId);

            var userDetails = await _userRepository.GetUserDetailsByIdAsync(request.UserId);

            if (userDetails == null)
            {
                throw new NotFoundException(nameof(User), request.UserId);
            }

            return userDetails;
        }
    }
}