using AutoMapper;
using FilmRentalStore.API.DTOs.Address;
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
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly Mock<ICountryRepository> _mockCountryRepository;
        private readonly Mock<ICityRepository> _mockCityRepository;
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockAddressRepository = new Mock<IAddressRepository>();
            _mockCountryRepository = new Mock<ICountryRepository>();
            _mockCityRepository = new Mock<ICityRepository>();
            _mockStoreRepository = new Mock<IStoreRepository>();
            _mockMapper = new Mock<IMapper>();

            _customerService = new CustomerService(
                _mockCustomerRepository.Object,
                _mockAddressRepository.Object,
                _mockCountryRepository.Object,
                _mockCityRepository.Object,
                _mockStoreRepository.Object,
                _mockMapper.Object
            );
        }

        // ============ POSITIVE TEST CASES ============

        /// <summary>
        /// POSITIVE TEST: Successfully retrieves paginated list of all customers
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WithValidPagination_ReturnsCustomerList()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", Username = "johndoe", Active = "Y" },
                new Customer { CustomerId = 2, FirstName = "Jane", LastName = "Smith", Username = "janesmith", Active = "Y" }
            };

            _mockCustomerRepository
                .Setup(r => r.GetAllAsync(page, pageSize))
                .ReturnsAsync((customers, 2));

            var customerDtos = new List<CustomerResponseDto>
            {
                new CustomerResponseDto { CustomerId = 1, FirstName = "John", LastName = "Doe" },
                new CustomerResponseDto { CustomerId = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _mockMapper
                .Setup(m => m.Map<IEnumerable<CustomerResponseDto>>(customers))
                .Returns(customerDtos);

            // Act
            var result = await _customerService.GetAllAsync(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockCustomerRepository.Verify(r => r.GetAllAsync(page, pageSize), Times.Once);
        }

        /// <summary>
        /// POSITIVE TEST: Successfully retrieves a customer by ID
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCustomer()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer 
            { 
                CustomerId = 1, 
                FirstName = "John", 
                LastName = "Doe", 
                Username = "johndoe",
                Email = "john@example.com",
                Active = "Y"
            };

            _mockCustomerRepository
                .Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            var customerDto = new CustomerResponseDto 
            { 
                CustomerId = 1, 
                FirstName = "John", 
                LastName = "Doe",
                Username = "johndoe"
            };

            _mockMapper
                .Setup(m => m.Map<CustomerResponseDto>(customer))
                .Returns(customerDto);

            // Act
            var result = await _customerService.GetByIdAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal("John", result.FirstName);
            _mockCustomerRepository.Verify(r => r.GetByIdAsync(customerId), Times.Once);
        }

        /// <summary>
        /// POSITIVE TEST: Successfully creates a new customer
        /// </summary>
        [Fact]
        public async Task CreateAsync_WithValidData_CreatesCustomer()
        {
            // Arrange
            var requestDto = new CustomerRequestDto
            {
                StoreId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "SecurePassword123",
                Email = "john@example.com",
                Address = new AddressRequestDto
                {
                    AddressLine = "123 Main St",
                    District = "Downtown",
                    Phone = "555-1234",
                    CityName = "New York",
                    CountryName = "United States"
                }
            };

            var createdCustomer = new Customer
            {
                CustomerId = 1,
                StoreId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                AddressId = 1,
                Active = "Y"
            };

            _mockStoreRepository
                .Setup(r => r.StoreExists(1))
                .ReturnsAsync(true);

            _mockCountryRepository
                .Setup(r => r.GetByNameAsync("United States"))
                .ReturnsAsync(new Country { CountryId = 1, Country1 = "United States" });

            _mockCityRepository
                .Setup(r => r.GetByNameAndCountryAsync("New York", 1))
                .ReturnsAsync(new City { CityId = 1, City1 = "New York", CountryId = 1 });

            _mockAddressRepository
                .Setup(r => r.CreateAsync(It.IsAny<Address>()))
                .ReturnsAsync(new Address { AddressId = 1 });

            _mockMapper
                .Setup(m => m.Map<Customer>(requestDto))
                .Returns(new Customer());

            _mockCustomerRepository
                .Setup(r => r.CreateAsync(It.IsAny<Customer>()))
                .ReturnsAsync(createdCustomer);

            _mockCustomerRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(createdCustomer);

            var responseDto = new CustomerResponseDto
            {
                CustomerId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe"
            };

            _mockMapper
                .Setup(m => m.Map<CustomerResponseDto>(createdCustomer))
                .Returns(responseDto);

            // Act
            var result = await _customerService.CreateAsync(requestDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            _mockCustomerRepository.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Once);
        }

        /// <summary>
        /// POSITIVE TEST: Successfully updates an existing customer
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesCustomer()
        {
            // Arrange
            var customerId = 1;
            var updateDto = new CustomerRequestDto
            {
                StoreId = 1,
                FirstName = "John",
                LastName = "Updated",
                Username = "johndoe",
                Email = "newemail@example.com"
            };

            var existingCustomer = new Customer
            {
                CustomerId = 1,
                StoreId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                AddressId = 1,
                Active = "Y"
            };

            _mockStoreRepository
                .Setup(r => r.StoreExists(1))
                .ReturnsAsync(true);

            _mockCustomerRepository
                .Setup(r => r.GetEntityByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _mockMapper.Setup(m => m.Map(updateDto, existingCustomer));

            _mockCustomerRepository
                .Setup(r => r.UpdateAsync(existingCustomer))
                .ReturnsAsync(existingCustomer);

            _mockCustomerRepository
                .Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            var responseDto = new CustomerResponseDto
            {
                CustomerId = 1,
                FirstName = "John",
                LastName = "Updated"
            };

            _mockMapper
                .Setup(m => m.Map<CustomerResponseDto>(existingCustomer))
                .Returns(responseDto);

            // Act
            var result = await _customerService.UpdateAsync(customerId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.LastName);
            _mockCustomerRepository.Verify(r => r.UpdateAsync(It.IsAny<Customer>()), Times.Once);
        }

        // ============ NEGATIVE TEST CASES ============

        /// <summary>
        /// NEGATIVE TEST: GetAllAsync throws NotFoundException when no customers found
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WithNoCustomers_ThrowsNotFoundException()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;

            _mockCustomerRepository
                .Setup(r => r.GetAllAsync(page, pageSize))
                .ReturnsAsync((new List<Customer>(), 0));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _customerService.GetAllAsync(page, pageSize)
            );

            Assert.Equal("No customers found.", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: GetByIdAsync throws NotFoundException when customer doesn't exist
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var customerId = 999;

            _mockCustomerRepository
                .Setup(r => r.GetByIdAsync(customerId))
                .ReturnsAsync((Customer?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _customerService.GetByIdAsync(customerId)
            );

            Assert.Contains("not found", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: CreateAsync throws BadRequestException when FirstName is empty
        /// </summary>
        [Fact]
        public async Task CreateAsync_WithEmptyFirstName_ThrowsBadRequestException()
        {
            // Arrange
            var requestDto = new CustomerRequestDto
            {
                StoreId = 1,
                FirstName = "",  // Invalid
                LastName = "Doe",
                Username = "johndoe",
                Password = "SecurePassword123"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _customerService.CreateAsync(requestDto)
            );

            Assert.Contains("FirstName and LastName are required", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: CreateAsync throws BadRequestException when password is too short
        /// </summary>
        [Fact]
        public async Task CreateAsync_WithShortPassword_ThrowsBadRequestException()
        {
            // Arrange
            var requestDto = new CustomerRequestDto
            {
                StoreId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "Short1"  // Less than 8 characters
            };

            _mockStoreRepository
                .Setup(r => r.StoreExists(1))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _customerService.CreateAsync(requestDto)
            );

            Assert.Contains("at least 8 characters", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: UpdateAsync throws NotFoundException when customer doesn't exist
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var customerId = 999;
            var updateDto = new CustomerRequestDto
            {
                StoreId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe"
            };

            _mockStoreRepository
                .Setup(r => r.StoreExists(1))
                .ReturnsAsync(true);

            _mockCustomerRepository
                .Setup(r => r.GetEntityByIdAsync(customerId))
                .ReturnsAsync((Customer?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => _customerService.UpdateAsync(customerId, updateDto)
            );

            Assert.Contains("not found", exception.Message);
        }

        /// <summary>
        /// NEGATIVE TEST: UpdateAsync throws ConflictException when username already exists
        /// </summary>
        [Fact]
        public async Task UpdateAsync_WithExistingUsername_ThrowsConflictException()
        {
            // Arrange
            var customerId = 1;
            var updateDto = new CustomerRequestDto
            {
                StoreId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "newusername"  // Different username
            };

            var existingCustomer = new Customer
            {
                CustomerId = 1,
                StoreId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                AddressId = 1,
                Active = "Y"
            };

            _mockStoreRepository
                .Setup(r => r.StoreExists(1))
                .ReturnsAsync(true);

            _mockCustomerRepository
                .Setup(r => r.GetEntityByIdAsync(customerId))
                .ReturnsAsync(existingCustomer);

            _mockCustomerRepository
                .Setup(r => r.UsernameExistsAsync("newusername", customerId))
                .ReturnsAsync(true);  // Username already exists

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => _customerService.UpdateAsync(customerId, updateDto)
            );

            Assert.Contains("already exists", exception.Message);
        }
    }
}
