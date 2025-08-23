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

        // GET api/users/userdata
        [HttpGet("userdata/{userId}")]
        public async Task<IActionResult> getUserById(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Phone,
                    u.RoleId
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Ok(user);
        }



        // PUT api/users/update/5
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] Data.User updatedUser)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.RoleId = updatedUser.RoleId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully" });
        }

        // PUT api/users/change-password/5
        [HttpPut("change-password/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // ⚠️ Normally you hash the password here before saving
            user.PasswordHash = newPassword;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully" });
        }

    }
}
