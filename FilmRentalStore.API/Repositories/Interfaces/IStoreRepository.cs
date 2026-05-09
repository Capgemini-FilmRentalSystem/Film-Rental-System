using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        Task<Store?> GetByIdAsync(int storeId);
        Task<bool> StoreExists(int storeId);
        Task<bool> ManagerStaffExistsAsync(int managerStaffId);
        Task<bool> AddressExistsAsync(int addressId);
        Task AddAsync(Store store);
        void Update(Store store);
        Task<bool> SaveChangesAsync();

    }
}
