using FilmRentalStore.MVC.DTOs.Staff;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IStaffApiService
    {
        Task<List<StaffResponseDto>> GetAllAsync();
        Task<StaffResponseDto?> GetByIdAsync(byte id);
        Task<StaffResponseDto?> CreateAsync(StaffCreateRequestDto dto);
        Task<StaffResponseDto?> UpdateAsync(byte id, StaffUpdateRequestDto dto);
        Task DeactivateAsync(byte id);
    }
}
