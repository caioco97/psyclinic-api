using Microsoft.IdentityModel.Tokens;
using PsyClinic.Infrasctructure.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PsyClinic.Api.Services
{
    public class JwtTokenService
    {
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly SigningCredentials _signingCredentials;
        private readonly int _expiresInMinutes;
        private static readonly JwtSecurityTokenHandler TokenHandler = new();

        public JwtTokenService(IConfiguration config)
        {
            _issuer = config["Jwt:Issuer"];
            _audience = config["Jwt:Audience"];
            _expiresInMinutes = int.Parse(config["Jwt:ExpiresMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiresInMinutes),
                signingCredentials: _signingCredentials
            );

            return TokenHandler.WriteToken(token);
        }
    }
}
