using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<IEnumerable<Staff>> GetAllAsync();
        Task<Staff?> GetByIdAsync(byte id);
        Task<Staff?> GetByIdWithDetailsAsync(byte id);
        Task<IEnumerable<Staff>> GetByStoreIdAsync(int storeId);
        Task<Staff?> GetByEmailAsync(string email);
        Task<Staff?> GetByUsernameAsync(string username);
        Task<Staff> CreateAsync(Staff staff);
        Task<Staff> UpdateAsync(Staff staff);
        Task DeleteAsync(Staff staff);
        Task<bool> ExistsAsync(byte id);
        Task<bool> EmailExistsAsync(string email, byte? excludeId = null);
        Task<bool> UsernameExistsAsync(string username, byte? excludeId = null);
        Task UpdateEmailAsync(byte id, string email);
        Task UpdatePasswordAsync(byte id, string hashedPassword);
        Task UpdateActiveStatusAsync(byte id, bool active);
        Task<int> CountByStoreAsync(int storeId);

    }
}
