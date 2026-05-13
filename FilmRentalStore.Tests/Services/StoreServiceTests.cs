using AutoMapper;
using FilmRentalStore.API.DTOs.Store;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class StoreServiceTests
{
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly StoreService _service;

    public StoreServiceTests()
    {
        _service = new StoreService(_storeRepository.Object, _mapper.Object);
    }

    [Fact]
    public async Task GetStoreByIdAsync_WhenStoreExists_ReturnsMappedStore()
    {
        var store = new Store { StoreId = 1, ManagerStaffId = 2, AddressId = 3 };
        var response = new StoreResponseDto { StoreId = 1 };

        _storeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(store);
        _mapper.Setup(m => m.Map<StoreResponseDto>(store)).Returns(response);

        var result = await _service.GetStoreByIdAsync(1);

        Assert.Equal(1, result.StoreId);
    }

    [Fact]
    public async Task CreateStoreAsync_WhenValidDto_AddsStoreAndReturnsCreatedStore()
    {
        var dto = new StoreRequestDto { ManagerStaffId = 2, AddressId = 3 };
        var store = new Store { ManagerStaffId = 2, AddressId = 3 };
        var response = new StoreResponseDto { StoreId = 4 };

        _storeRepository.Setup(r => r.ManagerStaffExistsAsync(2)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.ManagerAlreadyAssignedAsync(2, null)).ReturnsAsync(false);
        _storeRepository.Setup(r => r.AddressExistsAsync(3)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Store>(dto)).Returns(store);
        _storeRepository.Setup(r => r.AddAsync(store)).Callback<Store>(s => s.StoreId = 4).Returns(Task.CompletedTask);
        _storeRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _storeRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(store);
        _mapper.Setup(m => m.Map<StoreResponseDto>(store)).Returns(response);

        var result = await _service.CreateStoreAsync(dto);

        Assert.Equal(4, result.StoreId);
        _storeRepository.Verify(r => r.AddAsync(store), Times.Once);
    }

    [Fact]
    public async Task UpdateStoreAsync_WhenValidDto_UpdatesStoreAndReturnsUpdatedStore()
    {
        var dto = new StoreRequestDto { ManagerStaffId = 5, AddressId = 6 };
        var store = new Store { StoreId = 7, ManagerStaffId = 2, AddressId = 3 };
        var response = new StoreResponseDto { StoreId = 7 };

        _storeRepository.Setup(r => r.GetEntityByIdAsync(7)).ReturnsAsync(store);
        _storeRepository.Setup(r => r.ManagerStaffExistsAsync(5)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.ManagerAlreadyAssignedAsync(5, 7)).ReturnsAsync(false);
        _storeRepository.Setup(r => r.AddressExistsAsync(6)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<StoreRequestDto, Store>(dto, store)).Returns(store);
        _storeRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _storeRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(store);
        _mapper.Setup(m => m.Map<StoreResponseDto>(store)).Returns(response);

        var result = await _service.UpdateStoreAsync(7, dto);

        Assert.Equal(7, result.StoreId);
        _storeRepository.Verify(r => r.Update(store), Times.Once);
    }

    [Fact]
    public async Task CreateStoreAsync_WhenValidDto_SetsLastUpdate()
    {
        var dto = new StoreRequestDto { ManagerStaffId = 2, AddressId = 3 };
        var store = new Store { ManagerStaffId = 2, AddressId = 3 };
        var response = new StoreResponseDto { StoreId = 8 };

        _storeRepository.Setup(r => r.ManagerStaffExistsAsync(2)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.ManagerAlreadyAssignedAsync(2, null)).ReturnsAsync(false);
        _storeRepository.Setup(r => r.AddressExistsAsync(3)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Store>(dto)).Returns(store);
        _storeRepository.Setup(r => r.AddAsync(store)).Callback<Store>(s => s.StoreId = 8).Returns(Task.CompletedTask);
        _storeRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _storeRepository.Setup(r => r.GetByIdAsync(8)).ReturnsAsync(store);
        _mapper.Setup(m => m.Map<StoreResponseDto>(store)).Returns(response);

        await _service.CreateStoreAsync(dto);

        Assert.True(store.LastUpdate > DateTime.MinValue);
    }

    [Fact]
    public async Task GetStoreByIdAsync_WhenStoreDoesNotExist_ThrowsNotFoundException()
    {
        _storeRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Store?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetStoreByIdAsync(99));
    }

    [Fact]
    public async Task CreateStoreAsync_WhenDtoIsNull_ThrowsBadRequestException()
    {
        await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateStoreAsync(null!));
    }

    [Fact]
    public async Task CreateStoreAsync_WhenManagerAlreadyAssigned_ThrowsConflictException()
    {
        var dto = new StoreRequestDto { ManagerStaffId = 2, AddressId = 3 };

        _storeRepository.Setup(r => r.ManagerStaffExistsAsync(2)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.ManagerAlreadyAssignedAsync(2, null)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateStoreAsync(dto));
    }

    [Fact]
    public async Task UpdateStoreAsync_WhenAddressDoesNotExist_ThrowsBadRequestException()
    {
        var dto = new StoreRequestDto { ManagerStaffId = 5, AddressId = 999 };
        var store = new Store { StoreId = 7, ManagerStaffId = 2, AddressId = 3 };

        _storeRepository.Setup(r => r.GetEntityByIdAsync(7)).ReturnsAsync(store);
        _storeRepository.Setup(r => r.ManagerStaffExistsAsync(5)).ReturnsAsync(true);
        _storeRepository.Setup(r => r.ManagerAlreadyAssignedAsync(5, 7)).ReturnsAsync(false);
        _storeRepository.Setup(r => r.AddressExistsAsync(999)).ReturnsAsync(false);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateStoreAsync(7, dto));
    }
}
