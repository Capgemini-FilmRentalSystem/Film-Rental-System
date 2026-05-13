using BCrypt.Net;
using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using FilmRentalStore.API.Services.Interfaces;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IStaffRepository> _mockStaffRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockStaffRepository = new Mock<IStaffRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockAddressRepository = new Mock<IAddressRepository>();
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mockCustomerService = new Mock<ICustomerService>();
            _mockTokenService = new Mock<ITokenService>();

            _authService = new AuthService(
                _mockStaffRepository.Object,
                _mockCustomerRepository.Object,
                _mockAddressRepository.Object,
                _mockStoreRepository.Object,
                _mockCustomerService.Object,
                _mockTokenService.Object
            );
        }

        // ============ POSITIVE TEST CASES ============

        /// <summary>
        /// POSITIVE TEST: Successfully logs in staff with valid credentials
        /// </summary>
        [Fact]
        public async Task LoginStaffAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "staffuser",
                Password = "SecurePassword123"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("SecurePassword123");
            var staff = new Staff
            {
                StaffId = 1,
                Username = "staffuser",
                Password = hashedPassword,
                Active = true,
                Role = new Role { RoleId = 2, RoleTitle = "Manager" }
            };

            var loginResponse = new LoginResponseDto
            {
                Token = "jwt-token-here",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _mockStaffRepository
                .Setup(r => r.GetByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(staff);

            _mockTokenService
                .Setup(t => t.GenerateToken(staff))
                .Returns(loginResponse);

            // Act
            var result = await _authService.LoginStaffAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            _mockStaffRepository.Verify(r => r.GetByUsernameAsync(loginRequest.Username), Times.Once);
            _mockTokenService.Verify(t => t.GenerateToken(staff), Times.Once);
        }

        /// <summary>
        /// POSITIVE TEST: Successfully logs in customer with valid credentials
        /// </summary>
        [Fact]
        public async Task LoginCustomerAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "customeruser",
                Password = "SecurePassword123"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("SecurePassword123");
            var customer = new Customer
            {
                CustomerId = 1,
                Username = "customeruser",
                Password = hashedPassword,
                Active = "Y",
                Role = new Role { RoleId = 4, RoleTitle = "Customer" }
            };

            var loginResponse = new LoginResponseDto
            {
                Token = "jwt-token-here",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _mockCustomerRepository
                .Setup(r => r.GetByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(customer);

            _mockTokenService
                .Setup(t => t.GenerateToken(customer))
                .Returns(loginResponse);

            // Act
            var result = await _authService.LoginCustomerAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            _mockCustomerRepository.Verify(r => r.GetByUsernameAsync(loginRequest.Username), Times.Once);
            _mockTokenService.Verify(t => t.GenerateToken(customer), Times.Once);
        }

        /// <summary>
        /// POSITIVE TEST: Successfully registers a new staff member
        /// </summary>
        [Fact]
        public async Task RegisterStaffAsync_WithValidData_ReturnsLoginResponse()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                Username = "newstaff",
                Password = "SecurePassword123",
                FirstName = "John",
                LastName = "Manager",
                Email = "john@example.com",
                RoleId = 2,
                AddressId = 1,
                StoreId = 1
            };

            var address = new Address { AddressId = 1 };
            var createdStaff = new Staff
            {
                StaffId = 1,
                Username = "newstaff",
                FirstName = "John",
                LastName = "Manager",
                Active = true,
                Role = new Role { RoleId = 2, RoleTitle = "Manager" }
            };

            var loginResponse = new LoginResponseDto
            {
                Token = "jwt-token-here",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _mockStaffRepository
                .Setup(r => r.UsernameExistsAsync("newstaff"))
                .ReturnsAsync(false);

            _mockAddressRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(address);

            _mockStoreRepository
                .Setup(r => r.StoreExists(1))
                .ReturnsAsync(true);

            _mockStaffRepository
                .Setup(r => r.AddAsync(It.IsAny<Staff>()))
                .Returns(Task.CompletedTask);

            _mockStaffRepository
                .Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(true);

            _mockStaffRepository
                .Setup(r => r.GetByUsernameAsync("newstaff"))
                .ReturnsAsync(createdStaff);

            _mockTokenService
                .Setup(t => t.GenerateToken(createdStaff))
                .Returns(loginResponse);

            // Act
            var result = await _authService.RegisterStaffAsync(registerRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            _mockStaffRepository.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
        }

        /// <summary>
        /// POSITIVE TEST: Successfully registers a new customer
        /// </summary>
        [Fact]
        public async Task RegisterCustomerAsync_WithValidData_ReturnsLoginResponse()
        {
            // Arrange
            var registerRequest = new CustomerRequestDto
            {
                StoreId = 1,
                Username = "newcustomer",
                Password = "SecurePassword123",
                FirstName = "Jane",
                LastName = "Customer",
                Email = "jane@example.com"
            };

            var createdCustomer = new Customer
            {
                CustomerId = 1,
                Username = "newcustomer",
                FirstName = "Jane",
                LastName = "Customer",
                Active = "Y",
                Role = new Role { RoleId = 4, RoleTitle = "Customer" }
            };

            var loginResponse = new LoginResponseDto
            {
                Token = "jwt-token-here",
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            var customerResponseDto = new CustomerResponseDto
            {
                CustomerId = 1,
                Username = "newcustomer",
                FirstName = "Jane",
                LastName = "Customer",
                Email = "jane@example.com",
                StoreId = 1,
                IsActive = true
            };

            _mockCustomerService
                .Setup(s => s.RegisterAsync(registerRequest))
                .ReturnsAsync(customerResponseDto);

            _mockCustomerRepository
                .Setup(r => r.GetByUsernameAsync("newcustomer"))
                .ReturnsAsync(createdCustomer);

            _mockTokenService
                .Setup(t => t.GenerateToken(createdCustomer))
                .Returns(loginResponse);

            // Act
            var result = await _authService.RegisterCustomerAsync(registerRequest);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            _mockCustomerService.Verify(s => s.RegisterAsync(registerRequest), Times.Once);
        }

        // ============ NEGATIVE TEST CASES ============

        /// <summary>
        /// NEGATIVE TEST: LoginStaffAsync throws UnauthorizedException with invalid credentials
        /// </summary>
        [Fact]
        public async Task LoginStaffAsync_WithInvalidCredentials_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "staffuser",
                Password = "WrongPassword"
            };

            _mockStaffRepository
                .Setup(r => r.GetByUsernameAsync(loginRequest.Username))
                .ReturnsAsync((Staff?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginStaffAsync(loginRequest)
            );

            Assert.Equal("Invalid credentials.", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: LoginCustomerAsync throws UnauthorizedException with invalid credentials
        /// </summary>
        [Fact]
        public async Task LoginCustomerAsync_WithInvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "customeruser",
                Password = "WrongPassword"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("SecurePassword123");
            var customer = new Customer
            {
                CustomerId = 1,
                Username = "customeruser",
                Password = hashedPassword,
                Active = "Y",
                Role = new Role { RoleId = 4 }
            };

            _mockCustomerRepository
                .Setup(r => r.GetByUsernameAsync(loginRequest.Username))
                .ReturnsAsync(customer);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginCustomerAsync(loginRequest)
            );

            Assert.Equal("Invalid credentials.", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: RegisterStaffAsync throws ConflictException when username already exists
        /// </summary>
        [Fact]
        public async Task RegisterStaffAsync_WithExistingUsername_ThrowsConflictException()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                Username = "existingstaff",
                Password = "SecurePassword123",
                FirstName = "John",
                LastName = "Manager",
                RoleId = 2,
                AddressId = 1,
                StoreId = 1
            };

            _mockStaffRepository
                .Setup(r => r.UsernameExistsAsync("existingstaff"))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => _authService.RegisterStaffAsync(registerRequest)
            );

            Assert.Contains("already exists", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: RegisterStaffAsync throws BadRequestException with invalid role
        /// </summary>
        [Fact]
        public async Task RegisterStaffAsync_WithInvalidRole_ThrowsBadRequestException()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                Username = "newstaff",
                Password = "SecurePassword123",
                FirstName = "John",
                LastName = "Manager",
                RoleId = 5,  // Invalid role (must be 1-3)
                AddressId = 1,
                StoreId = 1
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _authService.RegisterStaffAsync(registerRequest)
            );

            Assert.Contains("RoleId must be 1, 2 or 3", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: LoginStaffAsync throws UnauthorizedException with null credentials
        /// </summary>
        [Fact]
        public async Task LoginStaffAsync_WithNullUsername_ThrowsUnauthorizedException()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "",
                Password = "SecurePassword123"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginStaffAsync(loginRequest)
            );

            Assert.Equal("Invalid credentials.", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: RegisterCustomerAsync throws BadRequestException when dto is null
        /// </summary>
        [Fact]
        public async Task RegisterCustomerAsync_WithNullDto_ThrowsBadRequestException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _authService.RegisterCustomerAsync(null!)
            );

            Assert.Contains("is required", exception.Message);
        }
    }
}
