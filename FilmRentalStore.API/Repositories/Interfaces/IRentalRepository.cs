using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IRentalRepository
    {
        Task<IEnumerable<Rental>> GetAllAsync(int page, int pageSize);

        Task<Rental?> GetByIdAsync(int rentalId);

        Task<Rental?> GetEntityByIdAsync(int rentalId);

        Task<Rental?> GetWithInventoryAsync(int rentalId);

        Task<bool> RentalExistsAsync(int rentalId);

        Task<bool> IsInventoryCurrentlyRentedAsync(int inventoryId);

        Task AddAsync(Rental rental);

        void Update(Rental rental);

        Task<bool> SaveChangesAsync();
    }
}
