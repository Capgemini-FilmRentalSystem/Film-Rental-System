using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        Task<IEnumerable<Store>> GetAllAsync();
        Task<Store?> GetByIdAsync(int id);
        Task<Store?> GetByIdWithDetailsAsync(int id);
        Task<Store> CreateAsync(Store store);
        Task<Store> UpdateAsync(Store store);
        Task DeleteAsync(Store store);
        Task<bool> ExistsAsync(int id);
        Task<Staff?> GetStoreManagerAsync(int storeId);
        Task UpdateManagerAsync(int storeId, byte managerStaffId);
        Task UpdateAddressAsync(int storeId, int addressId);
        Task<IEnumerable<Staff>> GetStaffByStoreAsync(int storeId);
        Task<IEnumerable<Models.Customer>> GetCustomersByStoreAsync(int storeId);
        Task<IEnumerable<StoreSalesSummary>> GetSalesByStoreAsync();
        Task<int> GetInventoryCountAsync(int storeId);

    }

    // ── Helper projection ─────────────────────────────────────────────────────
    public class StoreSalesSummary
    {
        public int StoreId { get; set; }
        public string Store { get; set; } = null!;
        public string Manager { get; set; } = null!;
        public decimal TotalSales { get; set; }
    }
}
