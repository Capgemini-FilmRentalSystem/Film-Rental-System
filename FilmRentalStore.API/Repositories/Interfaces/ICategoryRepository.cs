using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(byte id);
        Task<Category> AddAsync(Category category);
        void Update(Category category);
        Task<bool> ExistsAsync(byte id);
        Task<bool> NameExistsAsync(string name);
        Task<bool> SaveChangesAsync();
    }
}
