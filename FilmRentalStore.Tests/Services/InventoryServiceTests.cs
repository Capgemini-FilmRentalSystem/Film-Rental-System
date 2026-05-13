using AutoMapper;
using FilmRentalStore.API.DTOs.Inventory;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class InventoryServiceTests
{
    private readonly Mock<IInventoryRepository> _inventoryRepository = new();
    private readonly Mock<IFilmRepository> _filmRepository = new();
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _service = new InventoryService(
            _inventoryRepository.Object,
            _filmRepository.Object,
            _storeRepository.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task GetAllInventoryAsync_WhenInventoryExists_ReturnsMappedInventory()
    {
        var inventory = new List<Inventory> { new() { InventoryId = 1, FilmId = 10, StoreId = 1 } };
        var response = new List<InventoryResponseDto> { new() { InventoryId = 1 } };

        _inventoryRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync((inventory, inventory.Count));
        _mapper.Setup(m => m.Map<IEnumerable<InventoryResponseDto>>(inventory)).Returns(response);

        var result = await _service.GetAllInventoryAsync(1, 10);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetInventoryByIdAsync_WhenInventoryExists_ReturnsMappedInventory()
    {
        var inventory = new Inventory { InventoryId = 2, FilmId = 10, StoreId = 1 };
        var response = new InventoryResponseDto { InventoryId = 2 };

        _inventoryRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(inventory);
        _mapper.Setup(m => m.Map<InventoryResponseDto>(inventory)).Returns(response);

        var result = await _service.GetInventoryByIdAsync(2);

        Assert.Equal(2, result.InventoryId);
    }

    [Fact]
    public async Task CreateInventoryAsync_WhenValidDto_AddsInventoryAndReturnsCreatedInventory()
    {
        var dto = new InventoryRequestDto { FilmId = 10, StoreId = 1 };
        var inventory = new Inventory { InventoryId = 3, FilmId = 10, StoreId = 1 };
        var response = new InventoryResponseDto { InventoryId = 3 };

        _filmRepository.Setup(r => r.FilmExistsAsync(10)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.StoreExists(1)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Inventory>(dto)).Returns(inventory);
        _inventoryRepository.Setup(r => r.AddAsync(inventory)).Returns(Task.CompletedTask);
        _inventoryRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _inventoryRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(inventory);
        _mapper.Setup(m => m.Map<InventoryResponseDto>(inventory)).Returns(response);

        var result = await _service.CreateInventoryAsync(dto);

        Assert.Equal(3, result.InventoryId);
        _inventoryRepository.Verify(r => r.AddAsync(inventory), Times.Once);
    }

    [Fact]
    public async Task UpdateInventoryAsync_WhenValidDto_UpdatesInventoryAndReturnsUpdatedInventory()
    {
        var dto = new InventoryRequestDto { FilmId = 11, StoreId = 2 };
        var inventory = new Inventory { InventoryId = 4, FilmId = 10, StoreId = 1 };
        var response = new InventoryResponseDto { InventoryId = 4 };

        _inventoryRepository.Setup(r => r.GetEntityByIdAsync(4)).ReturnsAsync(inventory);
        _filmRepository.Setup(r => r.FilmExistsAsync(11)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.StoreExists(2)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<InventoryRequestDto, Inventory>(dto, inventory)).Returns(inventory);
        _inventoryRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _inventoryRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(inventory);
        _mapper.Setup(m => m.Map<InventoryResponseDto>(inventory)).Returns(response);

        var result = await _service.UpdateInventoryAsync(4, dto);

        Assert.Equal(4, result.InventoryId);
        _inventoryRepository.Verify(r => r.Update(inventory), Times.Once);
    }

    [Fact]
    public async Task GetAllInventoryAsync_WhenPageIsInvalid_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.GetAllInventoryAsync(0, 10));
    }

    [Fact]
    public async Task GetInventoryByIdAsync_WhenInventoryDoesNotExist_ThrowsNotFoundException()
    {
        _inventoryRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Inventory?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetInventoryByIdAsync(99));
    }

    [Fact]
    public async Task CreateInventoryAsync_WhenFilmDoesNotExist_ThrowsBadRequestException()
    {
        var dto = new InventoryRequestDto { FilmId = 999, StoreId = 1 };
        _filmRepository.Setup(r => r.FilmExistsAsync(999)).ReturnsAsync(false);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateInventoryAsync(dto));
    }

    [Fact]
    public async Task UpdateInventoryAsync_WhenStoreDoesNotExist_ThrowsBadRequestException()
    {
        var dto = new InventoryRequestDto { FilmId = 10, StoreId = 999 };
        var inventory = new Inventory { InventoryId = 5, FilmId = 10, StoreId = 1 };

        _inventoryRepository.Setup(r => r.GetEntityByIdAsync(5)).ReturnsAsync(inventory);
        _filmRepository.Setup(r => r.FilmExistsAsync(10)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.StoreExists(999)).ReturnsAsync(false);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateInventoryAsync(5, dto));
    }
}
