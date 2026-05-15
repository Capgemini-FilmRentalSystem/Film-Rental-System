using FilmRentalStore.API.DTOs.Staff;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffResponseDto>> GetAllStaffAsync();
        Task<IEnumerable<StaffResponseDto>> GetStaffForManagerStoreAsync(byte managerStaffId);
        Task<StaffResponseDto> GetStaffByIdAsync(byte staffId);
        Task<StaffResponseDto> GetStaffByIdForManagerStoreAsync(byte managerStaffId, byte staffId);
        Task<StaffResponseDto> CreateStaffAsync(StaffCreateRequestDto dto);
        Task<StaffResponseDto> UpdateStaffAsync(byte staffId, StaffUpdateRequestDto dto);
        Task DeactivateStaffAsync(byte staffId);
    }
}
