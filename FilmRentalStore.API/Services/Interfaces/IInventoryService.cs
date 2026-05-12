using FilmRentalStore.API.DTOs.Inventory;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IInventoryService
    {
        /// <summary>Returns all inventory items; throws NotFoundException when no inventory items exist.</summary>
        Task<IEnumerable<InventoryResponseDto>> GetAllInventoryAsync();

        Task<InventoryResponseDto> GetInventoryByIdAsync(int inventoryId);

        Task<InventoryResponseDto> CreateInventoryAsync(InventoryRequestDto inventoryDto);

        Task<InventoryResponseDto> UpdateInventoryAsync(int inventoryId, InventoryRequestDto inventoryDto);
    }
}
