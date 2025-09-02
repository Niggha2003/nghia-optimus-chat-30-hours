using System.Diagnostics.CodeAnalysis;

namespace Chat.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<Group> Groups { get; set; } = new List<Group>();

    }
}
