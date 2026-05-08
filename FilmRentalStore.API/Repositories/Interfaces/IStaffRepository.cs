using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<Staff> GetByIdAsync(byte staffId);
        Task<Staff> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> IsActiveAsync(byte staffId);
        Task<bool> IsAssignedToStore(byte staffid, int storeId);
        Task AddSync(Staff staff);
        void Update(Staff staff);
        void Deactivate(Staff staff);
        Task<bool> SaveChangesAsync();
    }
}
