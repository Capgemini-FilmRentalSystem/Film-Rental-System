using AutoMapper;
using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.DTOs.Customers;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<IAddressRepository> _addressRepository = new();
    private readonly Mock<ICountryRepository> _countryRepository = new();
    private readonly Mock<ICityRepository> _cityRepository = new();
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _service = new CustomerService(
            _customerRepository.Object,
            _addressRepository.Object,
            _countryRepository.Object,
            _cityRepository.Object,
            _storeRepository.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenCustomersExist_ReturnsMappedCustomers()
    {
        var customers = new List<Customer> { CreateCustomer(1) };
        var response = new List<CustomerResponseDto> { new() { CustomerId = 1, Username = "customer1" } };

        _customerRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync((customers, customers.Count));
        _mapper.Setup(m => m.Map<IEnumerable<CustomerResponseDto>>(customers)).Returns(response);

        var result = await _service.GetAllAsync(1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsMappedCustomer()
    {
        var customer = CreateCustomer(2);
        var response = new CustomerResponseDto { CustomerId = 2, Username = "customer2" };

        _customerRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(customer);
        _mapper.Setup(m => m.Map<CustomerResponseDto>(customer)).Returns(response);

        var result = await _service.GetByIdAsync(2);

        Assert.Equal(2, result.CustomerId);
    }

    [Fact]
    public async Task CreateAsync_WhenValidDto_CreatesCustomerAndReturnsCreatedCustomer()
    {
        var dto = CreateCustomerRequestDto();
        var country = new Country { CountryId = 1, Country1 = "India" };
        var city = new City { CityId = 2, City1 = "Mohali", CountryId = 1 };
        var createdAddress = new Address { AddressId = 3, Address1 = dto.Address!.AddressLine!, District = dto.Address.District!, Phone = dto.Address.Phone!, CityId = 2 };
        var createdCustomer = CreateCustomer(4);
        var response = new CustomerResponseDto { CustomerId = 4, Username = dto.Username! };

        _customerRepository.Setup(r => r.UsernameExistsAsync(dto.Username!, null)).ReturnsAsync(false);
        _storeRepository.Setup(r => r.StoreExists(dto.StoreId)).ReturnsAsync(true);
        _countryRepository.Setup(r => r.GetByNameAsync("India")).ReturnsAsync(country);
        _cityRepository.Setup(r => r.GetByNameAndCountryAsync("Mohali", 1)).ReturnsAsync(city);
        _addressRepository.Setup(r => r.CreateAsync(It.IsAny<Address>())).ReturnsAsync(createdAddress);
        _customerRepository.Setup(r => r.CreateAsync(It.IsAny<Customer>())).Callback<Customer>(c => c.CustomerId = 4).ReturnsAsync((Customer c) => c);
        _customerRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(createdCustomer);
        _mapper.Setup(m => m.Map<CustomerResponseDto>(createdCustomer)).Returns(response);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(4, result.CustomerId);
        _customerRepository.Verify(r => r.CreateAsync(It.Is<Customer>(c => c.RoleId == 4 && c.Active == "Y")), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenValidDto_UpdatesCustomerAndReturnsUpdatedCustomer()
    {
        var dto = CreateCustomerRequestDto();
        dto.Username = "updatedcustomer";
        dto.Password = "NewPassword@123";
        var address = dto.Address!;
        var existingCustomer = CreateCustomer(5);
        var updatedCustomer = CreateCustomer(5);
        updatedCustomer.Username = dto.Username;
        var response = new CustomerResponseDto { CustomerId = 5, Username = dto.Username! };

        _storeRepository.Setup(r => r.StoreExists(dto.StoreId)).ReturnsAsync(true);
        _customerRepository.Setup(r => r.GetEntityByIdAsync(5)).ReturnsAsync(existingCustomer);
        _customerRepository.Setup(r => r.UsernameExistsAsync(dto.Username!, 5)).ReturnsAsync(false);
        _addressRepository.Setup(r => r.GetByIdAsync(existingCustomer.AddressId)).ReturnsAsync(new Address { AddressId = existingCustomer.AddressId });
        _countryRepository.Setup(r => r.GetByNameAsync(address.CountryName!)).ReturnsAsync(new Country { CountryId = 1, Country1 = address.CountryName! });
        _cityRepository.Setup(r => r.GetByNameAndCountryAsync(address.CityName!, 1)).ReturnsAsync(new City { CityId = 2, City1 = address.CityName!, CountryId = 1 });
        _addressRepository.Setup(r => r.UpdateAsync(It.IsAny<Address>())).ReturnsAsync((Address a) => a);
        _mapper.Setup(m => m.Map<CustomerRequestDto, Customer>(dto, existingCustomer)).Returns(existingCustomer);
        _customerRepository.Setup(r => r.UpdateAsync(existingCustomer)).ReturnsAsync(updatedCustomer);
        _customerRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(updatedCustomer);
        _mapper.Setup(m => m.Map<CustomerResponseDto>(updatedCustomer)).Returns(response);

        var result = await _service.UpdateAsync(5, dto);

        Assert.Equal("updatedcustomer", result.Username);
        _customerRepository.Verify(r => r.UpdateAsync(existingCustomer), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenPageIsInvalid_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllAsync(0, 10));
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        _customerRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_WhenPasswordIsShort_ThrowsBadRequestException()
    {
        var dto = CreateCustomerRequestDto();
        dto.Password = "short";

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateAsync_WhenUsernameAlreadyExists_ThrowsConflictException()
    {
        var dto = CreateCustomerRequestDto();
        dto.Username = "duplicate";
        var existingCustomer = CreateCustomer(6);
        existingCustomer.Username = "oldusername";

        _storeRepository.Setup(r => r.StoreExists(dto.StoreId)).ReturnsAsync(true);
        _customerRepository.Setup(r => r.GetEntityByIdAsync(6)).ReturnsAsync(existingCustomer);
        _customerRepository.Setup(r => r.UsernameExistsAsync("duplicate", 6)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.UpdateAsync(6, dto));
    }

    private static CustomerRequestDto CreateCustomerRequestDto()
    {
        return new CustomerRequestDto
        {
            StoreId = 1,
            FirstName = "Test",
            LastName = "Customer",
            Email = "customer@test.com",
            Username = "customeruser",
            Password = "Password@123",
            Address = new AddressRequestDto
            {
                AddressLine = "123 Test Street",
                District = "Punjab",
                Phone = "9999999999",
                CityName = "Mohali",
                CountryName = "India"
            }
        };
    }

    private static Customer CreateCustomer(int id)
    {
        return new Customer
        {
            CustomerId = id,
            StoreId = 1,
            FirstName = "Test",
            LastName = "Customer",
            Email = "customer@test.com",
            AddressId = 3,
            Username = $"customer{id}",
            Active = "Y",
            RoleId = 4,
            CreateDate = DateTime.Now,
            LastUpdate = DateTime.Now
        };
    }
}
