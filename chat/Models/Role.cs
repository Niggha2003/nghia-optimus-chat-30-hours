namespace Chat.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    }
}
