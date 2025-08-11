using Microsoft.AspNetCore.Mvc;
using APiLoginWebApplication.Data;
using System.Linq;

namespace APiLoginWebApplication.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username && u.PasswordHash == request.Password);
            //var user1 = _context.Users;
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { message = "Login successful" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
