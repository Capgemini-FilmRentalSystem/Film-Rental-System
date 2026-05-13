using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using FilmRentalStore.API.Services.Interfaces;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IStaffRepository> _staffRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<IAddressRepository> _addressRepository = new();
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<ICustomerService> _customerService = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(
            _staffRepository.Object,
            _customerRepository.Object,
            _addressRepository.Object,
            _storeRepository.Object,
            _customerService.Object,
            _tokenService.Object);
    }

    [Fact]
    public async Task LoginStaffAsync_WhenCredentialsAreValid_ReturnsLoginResponse()
    {
        var dto = new LoginRequestDto { Username = "admin", Password = "Password@123" };
        var staff = new Staff
        {
            StaffId = 1,
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            Active = true,
            Role = new Role { RoleTitle = "Admin" }
        };
        var response = new LoginResponseDto { Token = "token", StaffId = 1, Username = "admin", Role = "Admin" };

        _staffRepository.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(staff);
        _tokenService.Setup(t => t.GenerateToken(staff)).Returns(response);

        var result = await _service.LoginStaffAsync(dto);

        Assert.Equal("token", result.Token);
        Assert.Equal("Admin", result.Role);
    }

    [Fact]
    public async Task LoginCustomerAsync_WhenCredentialsAreValid_ReturnsLoginResponse()
    {
        var dto = new LoginRequestDto { Username = "customer", Password = "Password@123" };
        var customer = new Customer
        {
            CustomerId = 10,
            Username = "customer",
            Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            Active = "Y",
            Role = new Role { RoleTitle = "Customer" }
        };
        var response = new LoginResponseDto { Token = "token", CustomerId = 10, Username = "customer", Role = "Customer" };

        _customerRepository.Setup(r => r.GetByUsernameAsync("customer")).ReturnsAsync(customer);
        _tokenService.Setup(t => t.GenerateToken(customer)).Returns(response);

        var result = await _service.LoginCustomerAsync(dto);

        Assert.Equal(10, result.CustomerId);
        Assert.Equal("Customer", result.Role);
    }

    [Fact]
    public async Task RegisterStaffAsync_WhenValidDto_CreatesStaffAndReturnsToken()
    {
        var dto = new RegisterRequestDto
        {
            FirstName = "New",
            LastName = "Staff",
            AddressId = 5,
            StoreId = 1,
            Username = "newstaff",
            Password = "Password@123",
            RoleId = 3
        };
        var createdStaff = new Staff
        {
            StaffId = 2,
            Username = dto.Username,
            Role = new Role { RoleTitle = "Staff" }
        };
        var response = new LoginResponseDto { Token = "staff-token", StaffId = 2, Username = dto.Username, Role = "Staff" };

        _staffRepository.Setup(r => r.UsernameExistsAsync(dto.Username)).ReturnsAsync(false);
        _addressRepository.Setup(r => r.GetByIdAsync(dto.AddressId)).ReturnsAsync(new Address { AddressId = dto.AddressId });
        _storeRepository.Setup(r => r.StoreExists(dto.StoreId)).ReturnsAsync(true);
        _staffRepository.Setup(r => r.AddAsync(It.IsAny<Staff>())).Returns(Task.CompletedTask);
        _staffRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _staffRepository.Setup(r => r.GetByUsernameAsync(dto.Username)).ReturnsAsync(createdStaff);
        _tokenService.Setup(t => t.GenerateToken(createdStaff)).Returns(response);

        var result = await _service.RegisterStaffAsync(dto);

        Assert.Equal("staff-token", result.Token);
        _staffRepository.Verify(r => r.AddAsync(It.Is<Staff>(s => s.Username == dto.Username && s.RoleId == 3)), Times.Once);
    }

    [Fact]
    public async Task RegisterCustomerAsync_WhenValidDto_DelegatesToCustomerServiceAndReturnsToken()
    {
        var dto = new CustomerRequestDto { Username = "newcustomer", Password = "Password@123", FirstName = "New", LastName = "Customer" };
        var customer = new Customer { CustomerId = 3, Username = dto.Username!, Role = new Role { RoleTitle = "Customer" } };
        var response = new LoginResponseDto { Token = "customer-token", CustomerId = 3, Username = dto.Username!, Role = "Customer" };

        _customerService.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(new CustomerResponseDto { CustomerId = 3, Username = dto.Username! });
        _customerRepository.Setup(r => r.GetByUsernameAsync(dto.Username!)).ReturnsAsync(customer);
        _tokenService.Setup(t => t.GenerateToken(customer)).Returns(response);

        var result = await _service.RegisterCustomerAsync(dto);

        Assert.Equal("customer-token", result.Token);
        _customerService.Verify(s => s.RegisterAsync(dto), Times.Once);
    }

    [Fact]
    public async Task LoginStaffAsync_WhenDtoIsNull_ThrowsUnauthorizedException()
    {
        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginStaffAsync(null!));
    }

    [Fact]
    public async Task LoginCustomerAsync_WhenPasswordIsWrong_ThrowsUnauthorizedException()
    {
        var dto = new LoginRequestDto { Username = "customer", Password = "wrong" };
        var customer = new Customer
        {
            CustomerId = 10,
            Username = "customer",
            Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
            Active = "Y",
            Role = new Role { RoleTitle = "Customer" }
        };

        _customerRepository.Setup(r => r.GetByUsernameAsync("customer")).ReturnsAsync(customer);

        await Assert.ThrowsAsync<UnauthorizedException>(() => _service.LoginCustomerAsync(dto));
    }

    [Fact]
    public async Task RegisterStaffAsync_WhenRoleIdIsInvalid_ThrowsBadRequestException()
    {
        var dto = new RegisterRequestDto { Username = "staff", Password = "Password@123", RoleId = 4 };

        await Assert.ThrowsAsync<BadRequestException>(() => _service.RegisterStaffAsync(dto));
    }

    [Fact]
    public async Task RegisterCustomerAsync_WhenCreatedCustomerCannotBeFound_ThrowsNotFoundException()
    {
        var dto = new CustomerRequestDto { Username = "customer", Password = "Password@123", FirstName = "New", LastName = "Customer" };

        _customerService.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(new CustomerResponseDto { Username = dto.Username! });
        _customerRepository.Setup(r => r.GetByUsernameAsync(dto.Username!)).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.RegisterCustomerAsync(dto));
    }
}
