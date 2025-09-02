namespace Chat.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionCode { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
