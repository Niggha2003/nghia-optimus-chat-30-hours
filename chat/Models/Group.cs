namespace Chat.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string GroupName { get; set; }
        public User User { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
