namespace Chat.Dtos.ViewDtos
{
    public class MessageViewDto
    {
        public string Id { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public bool IsGroup { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsViewed { get; set; } = false;
        public bool HasFile { get; set; }

    }
}
