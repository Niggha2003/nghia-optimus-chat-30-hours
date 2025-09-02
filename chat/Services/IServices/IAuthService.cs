using Chat.Models.APIModels;

namespace Chat.Services.IServices
{
    public interface IAuthService
    {
        Task<string?> Login(LoginRequest request);
        Task<string?> Refresh(string accessToken, string refreshToken);
    }
}
