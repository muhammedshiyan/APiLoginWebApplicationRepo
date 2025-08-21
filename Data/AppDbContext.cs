using APiLoginWebApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace APiLoginWebApplication.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }

    //public class User
    //{
    //    public int userid { get; set; }
    //    public string name { get; set; }
    //    public string PasswordHash { get; set; }
    //}

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Phone { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
