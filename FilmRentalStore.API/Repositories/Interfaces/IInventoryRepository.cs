using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAllAsync();

        Task<Inventory?> GetByIdAsync(int inventoryId);

        Task<Inventory?> GetEntityByIdAsync(int inventoryId);

        Task<bool> InventoryExistsAsync(int inventoryId);

        Task AddAsync(Inventory inventory);

        void Update(Inventory inventory);

        Task<bool> SaveChangesAsync();
    }
}
