using FilmRentalStore.API.DTOs.Staff;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IStaffService
    {
        Task<StaffResponseDto?> GetStaffByIdAsync(byte staffId);
        Task<StaffResponseDto> CreateStaffAsync(StaffCreateDto dto);
        Task<StaffResponseDto?> UpdateStaffAsync(byte staffId, StaffUpdateDto dto);
        Task DeactivateStaffAsync(byte staffId);
    }
}
