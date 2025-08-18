using APiLoginWebApplication.Models;
using System.Security.Claims;

namespace APiLoginWebApplication.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(string username);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        LoginResponse Login(LoginRequest request);
    }
}
