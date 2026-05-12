using FilmRentalStore.API.DTOs.Inventory;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IInventoryService
    {
        const int DefaultPage = 1;
        const int DefaultPageSize = 10;
        const int MaxPageSize = 100;

        /// <summary>Returns a paged inventory result; throws NotFoundException when no inventory items exist.</summary>
        Task<IEnumerable<InventoryResponseDto>> GetAllInventoryAsync(int page, int pageSize);

        Task<InventoryResponseDto> GetInventoryByIdAsync(int inventoryId);

        Task<InventoryResponseDto> CreateInventoryAsync(InventoryRequestDto inventoryDto);

        Task<InventoryResponseDto> UpdateInventoryAsync(int inventoryId, InventoryRequestDto inventoryDto);
    }
}
