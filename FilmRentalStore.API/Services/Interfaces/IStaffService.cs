using FilmRentalStore.API.DTOs.Staff;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffResponseDto>> GetAllAsync();
        Task<StaffDetailResponseDto> GetByIdAsync(byte id);
        Task<IEnumerable<StaffResponseDto>> GetByStoreIdAsync(int storeId);
        Task<StaffResponseDto> GetByEmailAsync(string email);
        Task<StaffResponseDto> CreateAsync(CreateStaffDto dto);
        Task<StaffResponseDto> UpdateAsync(byte id, UpdateStaffDto dto);
        Task DeleteAsync(byte id);
        Task UpdateEmailAsync(byte id, UpdateStaffEmailDto dto);
        Task UpdatePasswordAsync(byte id, UpdateStaffPasswordDto dto);
        Task UpdateActiveStatusAsync(byte id, UpdateStaffActiveDto dto);

    }
}
