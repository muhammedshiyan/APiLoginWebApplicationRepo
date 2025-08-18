using APiLoginWebApplication.Models;
using APiLoginWebApplication.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APiLoginWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = _authService.Login(request);
                return Ok(response);
            }
            catch
            {
                return Unauthorized("Invalid credentials");
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] TokenRequest request)
        {
            var principal = _authService.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                return BadRequest("Invalid token");

            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("Invalid username in token");

            // issue new tokens
            var newJwt = _authService.GenerateJwtToken(username);
            var newRefresh = _authService.GenerateRefreshToken();

            return Ok(new LoginResponse
            {
                Token = newJwt,
                RefreshToken = newRefresh
            });
        }
    }
}
