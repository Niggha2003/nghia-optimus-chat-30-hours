using Chat.Models.APIModels;
using Chat.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chat.Controllers
{

    // Controllers/AuthController.cs
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.Login(request);
            if (response == "401")
            {
                return Unauthorized();
            }
            if (response == "500")
            {
                return StatusCode(500, "Some thing wrong");
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenModel refreshTokenModel)
        {
            var response = await _authService.Refresh(refreshTokenModel.AccessToken, refreshTokenModel.RefreshToken);
            if (response == "401")
            {
                return Unauthorized();
            }
            if (response == "500")
            {
                return StatusCode(500, "Some thing wrong");
            }
            if (response == "400")
            {
                return StatusCode(400, "Bad request");
            }
            if (response == "404")
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet("IsValidAccess")]
        [Authorize]
        public async Task<IActionResult> CheckValidAccess()
        {
            return Ok(true);
        }

    }
}
