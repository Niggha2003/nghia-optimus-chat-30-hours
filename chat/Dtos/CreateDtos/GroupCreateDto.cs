namespace Chat.Dtos.CreateDtos
{
    public class GroupCreateDto
    {
        public int OwnerId { get; set; }
        public string GroupName { get; set; }
        public List<int> UserIds { get; set; }
    }
}
