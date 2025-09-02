namespace Chat.Models
{
    public class GroupSignalR
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public string UserConnectionId { get; set; }
        public string GroupGuid { get; set; }
    }
}
