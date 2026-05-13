using AutoMapper;
using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using Moq;
using Xunit;

namespace FilmRentalStore.API.Tests.Services;

public class StaffServiceTests
{
    private readonly Mock<IStaffRepository> _staffRepository = new();
    private readonly Mock<IAddressRepository> _addressRepository = new();
    private readonly Mock<IStoreRepository> _storeRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly StaffService _service;

    public StaffServiceTests()
    {
        _service = new StaffService(
            _staffRepository.Object,
            _addressRepository.Object,
            _storeRepository.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task GetStaffByIdAsync_WhenStaffExists_ReturnsMappedStaff()
    {
        var staff = new Staff { StaffId = 1, Username = "staff1", FirstName = "Staff", LastName = "One" };
        var response = new StaffResponseDto { StaffId = 1, Username = "staff1", FirstName = "Staff", LastName = "One" };

        _staffRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(staff);
        _mapper.Setup(m => m.Map<StaffResponseDto>(staff)).Returns(response);

        var result = await _service.GetStaffByIdAsync(1);

        Assert.Equal(1, result.StaffId);
    }

    [Fact]
    public async Task CreateStaffAsync_WhenValidDto_AddsStaffAndReturnsCreatedStaff()
    {
        var dto = CreateStaffCreateRequestDto();
        var staff = new Staff { FirstName = dto.FirstName, LastName = dto.LastName, Username = dto.Username, StoreId = dto.StoreId, AddressId = dto.AddressId };
        var response = new StaffResponseDto { StaffId = 2, Username = dto.Username, FirstName = dto.FirstName, LastName = dto.LastName };

        _staffRepository.Setup(r => r.UsernameExistsAsync(dto.Username)).ReturnsAsync(false);
        _addressRepository.Setup(r => r.GetByIdAsync(dto.AddressId)).ReturnsAsync(new Address { AddressId = dto.AddressId });
        _storeRepository.Setup(r => r.StoreExists(dto.StoreId)).ReturnsAsync(true);
        _mapper.Setup(m => m.Map<Staff>(dto)).Returns(staff);
        _staffRepository.Setup(r => r.AddAsync(staff)).Callback<Staff>(s => s.StaffId = 2).Returns(Task.CompletedTask);
        _staffRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _staffRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(staff);
        _mapper.Setup(m => m.Map<StaffResponseDto>(staff)).Returns(response);

        var result = await _service.CreateStaffAsync(dto);

        Assert.Equal(2, result.StaffId);
        _staffRepository.Verify(r => r.AddAsync(staff), Times.Once);
    }

    [Fact]
    public async Task UpdateStaffAsync_WhenValidDto_UpdatesStaffAndReturnsUpdatedStaff()
    {
        var dto = new StaffUpdateRequestDto { FirstName = "Updated", LastName = "Staff", AddressId = 5, Active = true, RoleId = 2 };
        var staff = new Staff { StaffId = 3, Username = "staff3", AddressId = 5 };
        var response = new StaffResponseDto { StaffId = 3, Username = "staff3", FirstName = "Updated", LastName = "Staff" };

        _staffRepository.Setup(r => r.GetEntityByIdAsync(3)).ReturnsAsync(staff);
        _addressRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(new Address { AddressId = 5 });
        _mapper.Setup(m => m.Map<StaffUpdateRequestDto, Staff>(dto, staff)).Returns(staff);
        _staffRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _staffRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(staff);
        _mapper.Setup(m => m.Map<StaffResponseDto>(staff)).Returns(response);

        var result = await _service.UpdateStaffAsync(3, dto);

        Assert.Equal(3, result.StaffId);
        _staffRepository.Verify(r => r.Update(staff), Times.Once);
    }

    [Fact]
    public async Task DeactivateStaffAsync_WhenStaffExists_DeactivatesStaff()
    {
        var staff = new Staff { StaffId = 4, Active = true };

        _staffRepository.Setup(r => r.GetEntityByIdAsync(4)).ReturnsAsync(staff);
        _staffRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        await _service.DeactivateStaffAsync(4);

        _staffRepository.Verify(r => r.Deactivate(staff), Times.Once);
        _staffRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetStaffByIdAsync_WhenStaffDoesNotExist_ThrowsNotFoundException()
    {
        _staffRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Staff?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetStaffByIdAsync(99));
    }

    [Fact]
    public async Task CreateStaffAsync_WhenUsernameAlreadyExists_ThrowsConflictException()
    {
        var dto = CreateStaffCreateRequestDto();
        _staffRepository.Setup(r => r.UsernameExistsAsync(dto.Username)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateStaffAsync(dto));
    }

    [Fact]
    public async Task UpdateStaffAsync_WhenAddressDoesNotExist_ThrowsBadRequestException()
    {
        var dto = new StaffUpdateRequestDto { FirstName = "Updated", LastName = "Staff", AddressId = 99, Active = true, RoleId = 2 };
        var staff = new Staff { StaffId = 5, Username = "staff5" };

        _staffRepository.Setup(r => r.GetEntityByIdAsync(5)).ReturnsAsync(staff);
        _addressRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Address?)null);

        await Assert.ThrowsAsync<BadRequestException>(() => _service.UpdateStaffAsync(5, dto));
    }

    [Fact]
    public async Task DeactivateStaffAsync_WhenStaffDoesNotExist_ThrowsNotFoundException()
    {
        _staffRepository.Setup(r => r.GetEntityByIdAsync(100)).ReturnsAsync((Staff?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeactivateStaffAsync(100));
    }

    private static StaffCreateRequestDto CreateStaffCreateRequestDto()
    {
        return new StaffCreateRequestDto
        {
            FirstName = "Test",
            LastName = "Staff",
            AddressId = 5,
            Email = "staff@test.com",
            StoreId = 1,
            Username = "staffuser",
            Password = "Password@123",
            RoleId = 2
        };
    }
}
