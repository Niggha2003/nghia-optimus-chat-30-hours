namespace Chat.Dtos.CreateDtos
{
    public class UserCreateDto
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public List<int> RoleIds { get; set; }
    }
}
