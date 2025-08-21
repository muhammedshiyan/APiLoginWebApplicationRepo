namespace APiLoginWebApplication.Models
{
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
    //public class User
    //{
    //    public int Id { get; set; }
    //    public string Username { get; set; } = string.Empty;
    //    public string PasswordHash { get; set; } = string.Empty; // Store hashed passwords
    //    public string Role { get; set; } = "User"; // Default role
    //}
}
