using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FilmRentalStore.API.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LoginResponseDto GenerateToken(Staff staff)
        {
            var key       = _configuration["Jwt:Key"]!;
            var issuer    = _configuration["Jwt:Issuer"]!;
            var audience  = _configuration["Jwt:Audience"]!;
            var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,    staff.StaffId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, staff.Username),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,               staff.Role.RoleTitle)
            };

            var signingKey   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials  = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiresAt    = DateTime.UtcNow.AddMinutes(expiresIn);

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                expires:            expiresAt,
                signingCredentials: credentials
            );

            return new LoginResponseDto
            {
                Token     = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                StaffId   = staff.StaffId,
                Username  = staff.Username,
                Role      = staff.Role.RoleTitle
            };
        }
    }
}