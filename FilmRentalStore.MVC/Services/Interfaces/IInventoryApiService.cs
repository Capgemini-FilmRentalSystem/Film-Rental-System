using FilmRentalStore.MVC.DTOs.Inventory;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IInventoryApiService
    {
        Task<List<InventoryResponseDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<InventoryResponseDto?> GetByIdAsync(int id);
        Task<InventoryResponseDto?> CreateAsync(InventoryRequestDto dto);
        Task<InventoryResponseDto?> UpdateAsync(int id, InventoryRequestDto dto);
    }
}
