using Chat.Models;
using System.Security.Claims;

namespace Chat.Services.IServices
{
    public interface ITokenService
    {
        string CreateToken(User userInfo);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
