using FilmRentalStore.API.DTOs.Staff;
using FilmRentalStore.API.DTOs.Store;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IStoreService
    {
        Task<IEnumerable<StoreResponseDto>> GetAllAsync();
        Task<StoreDetailResponseDto> GetByIdAsync(int id);
        Task<StoreResponseDto> CreateAsync(CreateStoreDto dto);
        Task<StoreResponseDto> UpdateAsync(int id, UpdateStoreDto dto);
        Task DeleteAsync(int id);
        Task<StaffResponseDto> GetStoreManagerAsync(int storeId);
        Task UpdateStoreManagerAsync(int storeId, UpdateStoreManagerDto dto);
        Task UpdateStoreAddressAsync(int storeId, UpdateStoreAddressDto dto);
        Task<IEnumerable<StoreStaffSummaryDto>> GetStoreStaffAsync(int storeId);
        Task<IEnumerable<StoreCustomerSummaryDto>> GetStoreCustomersAsync(int storeId);
        Task<IEnumerable<StoreSalesDto>> GetSalesByStoreAsync();
        Task<int> GetInventoryCountAsync(int storeId);

    }
}
