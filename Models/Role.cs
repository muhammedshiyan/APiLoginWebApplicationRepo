namespace APiLoginWebApplication.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public virtual  ICollection<User>? Users { get; set; }
    }
}
