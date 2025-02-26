using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces
{
    public interface IJwtService
    {
        (string Token, DateTime ExpiresAt) GenerateJwtToken(Guid userId, string email, List<string> roles);
        (string Token, DateTime ExpiresAt) GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
