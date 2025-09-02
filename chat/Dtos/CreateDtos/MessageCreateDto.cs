namespace Chat.Dtos.CreateDtos
{
    public class MessageCreateDto
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public bool IsGroup { get; set; }
        public string Content { get; set; }
    }
}
