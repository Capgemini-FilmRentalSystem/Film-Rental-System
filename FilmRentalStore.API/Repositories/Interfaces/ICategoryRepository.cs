using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(byte id);
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetCategoriesWithFilmCountAsync();
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> ExistsAsync(byte id);
        Task<bool> NameExistsAsync(string name, byte? excludeId = null);
        Task<bool> HasFilmsAsync(byte id);
    }
}
