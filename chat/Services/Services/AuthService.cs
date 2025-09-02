using AutoMapper;
using Chat.Contexts;
using Chat.Controllers;
using Chat.Models.APIModels;
using Chat.Repository;
using Chat.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Chat.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(AppDbContext context, ITokenService tokenService, IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<string?> Login(LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                var isValid = await _userRepository.CheckUserValid(request.Email, request.Password);
                if (isValid != null)
                {
                    _logger.LogInformation("User validated successfully for email: {Email}", request.Email);

                    var token = _tokenService.CreateToken(isValid);
                    var refreshToken = _tokenService.GenerateRefreshToken();

                    isValid.AccessToken = token;
                    isValid.RefreshToken = refreshToken;
                    isValid.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Login successful for email: {Email}", request.Email);
                    var result = new RefreshTokenModel {
                        AccessToken = token,
                        RefreshToken = refreshToken,
                    };

                    return JsonConvert.SerializeObject(result);
                }

                _logger.LogWarning("Unauthorized login attempt for email: {Email}", request.Email);
                return "401";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login for email: {Email}", request.Email);
                return "500";
            }
        }

        public async Task<string?> Refresh(string accessToken, string refreshToken)
        {
            try
            {
                _logger.LogInformation("Attempting to refresh token.");

                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
                var id = principal.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Invalid access token: missing user ID claim.");
                    return "400";
                }

                var user = await _context.Users.FindAsync(int.Parse(id));
                if (user == null)
                {
                    _logger.LogWarning("Refresh failed: user with ID {UserId} not found.", id);
                    return "404";
                }

                if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh failed: invalid refresh token or token expired for user ID {UserId}.", id);
                    return "401";
                }

                var newAccessToken = _tokenService.CreateToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                _context.Entry(user).State = EntityState.Modified;
                user.AccessToken = newAccessToken;
                user.RefreshToken = newRefreshToken;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Token refreshed successfully for user ID {UserId}.", id);
                var result = new RefreshTokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                };

                return JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh.");
                return "500";
            }
        }
    }
}