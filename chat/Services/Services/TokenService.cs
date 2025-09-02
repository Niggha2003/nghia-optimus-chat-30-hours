
using Chat.Contexts;
using Chat.Models;
using Chat.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Chat.Services
{

    public class TokenService : ITokenService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public TokenService(AppDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        public string CreateToken(User userInfo)
        {
            // Kiểm tra thông tin người dùng
            var authorInfo = _context.Users.Where(x => x.Id == userInfo.Id).FirstOrDefault();

            // Lấy secret key từ cấu hình
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tạo các "claims" - thông tin về user
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserEmail), // Subject
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID
            //  có thể thêm các claims tùy chỉnh ở đây, ví dụ: role
            new Claim("id", userInfo.Id.ToString()),
            new Claim(ClaimTypes.Email, userInfo.UserEmail),
            new Claim(ClaimTypes.Name, userInfo.UserName),
        };

            foreach (var role in userInfo.Roles.ToList())
            {
                claims.Add(new Claim(ClaimTypes.Role, role.RoleCode));
            }

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(100), // Thời gian hết hạn
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                // Xác thực khóa ký
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),

                // Xác thực Issuer (tổ chức phát hành)
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],

                // Xác thực Audience (đối tượng sử dụng)
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],

                ValidateLifetime = false, // bỏ qua kiểm tra thời gian sống
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

    }
}
