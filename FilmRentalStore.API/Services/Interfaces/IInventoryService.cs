using FilmRentalStore.API.DTOs.Inventory;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryResponseDto>> GetAllInventoryAsync();

        Task<InventoryResponseDto> GetInventoryByIdAsync(int inventoryId);

        Task<InventoryResponseDto> CreateInventoryAsync(InventoryDto inventoryDto);

        Task<InventoryResponseDto> UpdateInventoryAsync(int inventoryId, InventoryDto inventoryDto);
    }
}