using APiLoginWebApplication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace APiLoginWebApplication.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.EmailId && u.PasswordHash == request.password);
            //var user1 = _context.Users;
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { message = "Login successful" });
        }


    }

    public class LoginRequest
    {
        public string EmailId { get; set; }
        public string password { get; set; }
    }
}
