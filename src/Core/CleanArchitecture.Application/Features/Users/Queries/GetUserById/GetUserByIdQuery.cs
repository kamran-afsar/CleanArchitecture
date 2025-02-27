using AutoMapper;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Features.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.Id);
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}