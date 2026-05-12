using FilmRentalStore.API.DTOs.Staff;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IStaffService
    {
        Task<StaffResponseDto> GetStaffByIdAsync(byte staffId);
        Task<StaffResponseDto> CreateStaffAsync(StaffCreateRequestDto dto);
        Task<StaffResponseDto> UpdateStaffAsync(byte staffId, StaffUpdateRequestDto dto);
        Task DeactivateStaffAsync(byte staffId);
    }
}
