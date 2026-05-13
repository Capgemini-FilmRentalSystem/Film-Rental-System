using AutoMapper;
using FilmRentalStore.API.DTOs.Rental;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _rentalRepository = new();
    private readonly Mock<IInventoryRepository> _inventoryRepository = new();
    private readonly Mock<ICustomerRepository> _customerRepository = new();
    private readonly Mock<IStaffRepository> _staffRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _service = new RentalService(
            _rentalRepository.Object,
            _inventoryRepository.Object,
            _customerRepository.Object,
            _staffRepository.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task GetAllRentalsAsync_WhenRentalsExist_ReturnsMappedRentals()
    {
        var rentals = new List<Rental> { new() { RentalId = 1, CustomerId = 1, InventoryId = 1, StaffId = 1 } };
        var response = new List<RentalResponseDto> { new() { RentalId = 1 } };

        _rentalRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(rentals);
        _mapper.Setup(m => m.Map<IEnumerable<RentalResponseDto>>(rentals)).Returns(response);

        var result = await _service.GetAllRentalsAsync(1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetRentalByIdAsync_WhenRentalExists_ReturnsMappedRental()
    {
        var rental = new Rental { RentalId = 2, CustomerId = 1, InventoryId = 1, StaffId = 1 };
        var response = new RentalResponseDto { RentalId = 2 };

        _rentalRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(rental);
        _mapper.Setup(m => m.Map<RentalResponseDto>(rental)).Returns(response);

        var result = await _service.GetRentalByIdAsync(2);

        Assert.Equal(2, result.RentalId);
    }

    [Fact]
    public async Task CreateRentalAsync_WhenValidDto_AddsRentalAndReturnsCreatedRental()
    {
        var dto = new RentalRequestDto { InventoryId = 5, CustomerId = 7, StaffId = 2 };
        var inventory = new Inventory { InventoryId = 5, StoreId = 1 };
        var rental = new Rental { RentalId = 3, InventoryId = 5, CustomerId = 7, StaffId = 2 };
        var response = new RentalResponseDto { RentalId = 3 };

        _inventoryRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(inventory);
        _customerRepository.Setup(r => r.ExistsAsync(7)).ReturnsAsync(true);
        _staffRepository.Setup(r => r.IsActiveAsync(2)).ReturnsAsync(true);
        _staffRepository.Setup(r => r.IsAssignedToStore(2, 1)).ReturnsAsync(true);
        _rentalRepository.Setup(r => r.IsInventoryCurrentlyRentedAsync(5)).ReturnsAsync(false);
        _mapper.Setup(m => m.Map<Rental>(dto)).Returns(rental);
        _rentalRepository.Setup(r => r.AddAsync(rental)).Returns(Task.CompletedTask);
        _rentalRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _rentalRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(rental);
        _mapper.Setup(m => m.Map<RentalResponseDto>(rental)).Returns(response);

        var result = await _service.CreateRentalAsync(dto);

        Assert.Equal(3, result.RentalId);
        _rentalRepository.Verify(r => r.AddAsync(rental), Times.Once);
    }

    [Fact]
    public async Task ReturnRentalAsync_WhenRentalIsOpen_ReturnsUpdatedRental()
    {
        var rental = new Rental { RentalId = 4, RentalDate = DateTime.Now.AddDays(-1), ReturnDate = null };
        var dto = new RentalReturnRequestDto { ReturnDate = DateTime.Now };
        var response = new RentalResponseDto { RentalId = 4, ReturnDate = dto.ReturnDate };

        _rentalRepository.Setup(r => r.GetEntityByIdAsync(4)).ReturnsAsync(rental);
        _rentalRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _rentalRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(rental);
        _mapper.Setup(m => m.Map<RentalResponseDto>(rental)).Returns(response);

        var result = await _service.ReturnRentalAsync(4, dto);

        Assert.Equal(4, result.RentalId);
        _rentalRepository.Verify(r => r.Update(rental), Times.Once);
    }

    [Fact]
    public async Task GetAllRentalsAsync_WhenPageIsInvalid_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllRentalsAsync(0, 10));
    }

    [Fact]
    public async Task GetRentalByIdAsync_WhenRentalDoesNotExist_ThrowsNotFoundException()
    {
        _rentalRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Rental?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetRentalByIdAsync(99));
    }

    [Fact]
    public async Task CreateRentalAsync_WhenInventoryAlreadyRented_ThrowsConflictException()
    {
        var dto = new RentalRequestDto { InventoryId = 5, CustomerId = 7, StaffId = 2 };
        var inventory = new Inventory { InventoryId = 5, StoreId = 1 };

        _inventoryRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(inventory);
        _customerRepository.Setup(r => r.ExistsAsync(7)).ReturnsAsync(true);
        _staffRepository.Setup(r => r.IsActiveAsync(2)).ReturnsAsync(true);
        _staffRepository.Setup(r => r.IsAssignedToStore(2, 1)).ReturnsAsync(true);
        _rentalRepository.Setup(r => r.IsInventoryCurrentlyRentedAsync(5)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateRentalAsync(dto));
    }

    [Fact]
    public async Task ReturnRentalAsync_WhenReturnDateIsBeforeRentalDate_ThrowsBadRequestException()
    {
        var rental = new Rental { RentalId = 6, RentalDate = DateTime.Now, ReturnDate = null };
        var dto = new RentalReturnRequestDto { ReturnDate = DateTime.Now.AddDays(-1) };

        _rentalRepository.Setup(r => r.GetEntityByIdAsync(6)).ReturnsAsync(rental);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.ReturnRentalAsync(6, dto));
    }
}
