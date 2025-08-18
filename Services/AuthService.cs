using APiLoginWebApplication.Data;
using APiLoginWebApplication.Models;
using APiLoginWebApplication.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace APiLoginWebApplication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private static Dictionary<string, string> _refreshTokens = new();
        private readonly AppDbContext _context;
        public AuthService(AppDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public LoginResponse Login(LoginRequest request)
        {
            // Fake validation (replace with DB validation)


            var user = _context.Users.FirstOrDefault(u => u.Username == request.EmailId && u.PasswordHash == request.password);
            //var user1 = _context.Users;
            if (user == null)
                throw new ArgumentException("User ID must be a positive number.");


            if (user!=null)
            {
                var token = GenerateJwtToken(request.EmailId);
                var refreshToken = GenerateRefreshToken();

                _refreshTokens[request.EmailId] = refreshToken;

                return new LoginResponse
                {
                    Token = token,
                    RefreshToken = refreshToken
                };
            }

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        public string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!)),
                ValidateLifetime = false // ignore expiration
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
