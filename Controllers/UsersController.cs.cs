using APiLoginWebApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APiLoginWebApplication.Models;


namespace APiLoginWebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context) => _context = context;
        // POST api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(Data.User user)
        {
            if (await _context.Users.AnyAsync(u => u.Name == user.Name))
                return BadRequest("name already exists");

            // hash password (for demo only, use BCrypt/Identity in production)
            user.PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(user.PasswordHash));
           // user.RoleId = 1;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered successfully" });
        }

        // GET api/users/roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new { r.RoleId, r.RoleName })
                .ToListAsync();

            return Ok(roles);
        }
    }
}
