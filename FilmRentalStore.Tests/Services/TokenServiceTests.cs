using FilmRentalStore.API.Models;
using FilmRentalStore.API.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly TokenService _service;

    public TokenServiceTests()
    {
        SetupValidJwtConfiguration();
        _service = new TokenService(_configuration.Object);
    }

    [Fact]
    public void GenerateToken_WhenStaffIsValid_ReturnsTokenWithStaffDetails()
    {
        var staff = new Staff
        {
            StaffId = 1,
            Username = "admin",
            Role = new Role { RoleTitle = "Admin" }
        };

        var result = _service.GenerateToken(staff);

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal((byte?)1, result.StaffId);
        Assert.Equal("admin", result.Username);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public void GenerateToken_WhenCustomerIsValid_ReturnsTokenWithCustomerDetails()
    {
        var customer = new Customer
        {
            CustomerId = 10,
            Username = "customer",
            Role = new Role { RoleTitle = "Customer" }
        };

        var result = _service.GenerateToken(customer);

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal(10, result.CustomerId);
        Assert.Equal("customer", result.Username);
        Assert.Equal("Customer", result.Role);
    }

    [Fact]
    public void GenerateToken_WhenStaffIsValid_TokenContainsRoleClaim()
    {
        var staff = new Staff
        {
            StaffId = 2,
            Username = "manager",
            Role = new Role { RoleTitle = "Manager" }
        };

        var result = _service.GenerateToken(staff);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Manager");
    }

    [Fact]
    public void GenerateToken_WhenCustomerIsValid_TokenContainsCustomerIdClaim()
    {
        var customer = new Customer
        {
            CustomerId = 20,
            Username = "customer20",
            Role = new Role { RoleTitle = "Customer" }
        };

        var result = _service.GenerateToken(customer);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

        Assert.Contains(token.Claims, c => c.Type == "customer_id" && c.Value == "20");
    }

    [Fact]
    public void GenerateToken_WhenJwtKeyIsMissing_ThrowsException()
    {
        _configuration.Setup(c => c["Jwt:Key"]).Returns((string?)null);
        var service = new TokenService(_configuration.Object);
        var staff = new Staff { StaffId = 1, Username = "admin", Role = new Role { RoleTitle = "Admin" } };

        Assert.Throws<ArgumentNullException>(() => service.GenerateToken(staff));
    }

    [Fact]
    public void GenerateToken_WhenExpiresInMinutesIsInvalid_ThrowsFormatException()
    {
        _configuration.Setup(c => c["Jwt:ExpiresInMinutes"]).Returns("invalid");
        var service = new TokenService(_configuration.Object);
        var staff = new Staff { StaffId = 1, Username = "admin", Role = new Role { RoleTitle = "Admin" } };

        Assert.Throws<FormatException>(() => service.GenerateToken(staff));
    }

    [Fact]
    public void GenerateToken_WhenStaffRoleIsMissing_ThrowsNullReferenceException()
    {
        var staff = new Staff { StaffId = 1, Username = "admin", Role = null! };

        Assert.Throws<NullReferenceException>(() => _service.GenerateToken(staff));
    }

    [Fact]
    public void GenerateToken_WhenCustomerUsernameIsMissing_ThrowsArgumentNullException()
    {
        var customer = new Customer { CustomerId = 10, Username = null!, Role = new Role { RoleTitle = "Customer" } };

        Assert.Throws<ArgumentNullException>(() => _service.GenerateToken(customer));
    }

    private void SetupValidJwtConfiguration()
    {
        _configuration.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretJwtKeyWithAtLeastThirtyTwoCharacters123");
        _configuration.Setup(c => c["Jwt:Issuer"]).Returns("FilmRentalStore.Tests");
        _configuration.Setup(c => c["Jwt:Audience"]).Returns("FilmRentalStore.Tests.Client");
        _configuration.Setup(c => c["Jwt:ExpiresInMinutes"]).Returns("60");
    }
}
