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
            var expiresAt = GetExpiresAt();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,    staff.StaffId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, staff.Username),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,               staff.Role.RoleTitle)
            };

            var token = CreateToken(claims, expiresAt);

            return new LoginResponseDto
            {
                Token     = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                StaffId   = staff.StaffId,
                Username  = staff.Username,
                Role      = staff.Role.RoleTitle
            };
        }

        public LoginResponseDto GenerateToken(Customer customer)
        {
            var expiresAt = GetExpiresAt();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.CustomerId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, customer.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, customer.Role.RoleTitle),
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                new Claim("customer_id", customer.CustomerId.ToString())
            };

            var token = CreateToken(claims, expiresAt);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                CustomerId = customer.CustomerId,
                Username = customer.Username,
                Role = customer.Role.RoleTitle
            };
        }

        private JwtSecurityToken CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
        {
            var key = _configuration["Jwt:Key"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );
        }

        private DateTime GetExpiresAt()
        {
            var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");
            return DateTime.UtcNow.AddMinutes(expiresIn);
        }
    }
}
