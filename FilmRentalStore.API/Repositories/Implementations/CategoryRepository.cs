using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<Category?> GetByIdAsync(byte id)
            => await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);

        public async Task<Category?> GetByNameAsync(string name)
            => await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == name);

        public async Task<IEnumerable<Category>> GetCategoriesWithFilmCountAsync()
            => await _context.Categories
                .Include(c => c.FilmCategories)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<Category> CreateAsync(Category category)
        {
            category.LastUpdate = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            category.LastUpdate = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(byte id)
            => await _context.Categories.AnyAsync(c => c.CategoryId == id);

        public async Task<bool> NameExistsAsync(string name, byte? excludeId = null)
            => await _context.Categories.AnyAsync(c =>
                c.Name == name && (excludeId == null || c.CategoryId != excludeId));

        public async Task<bool> HasFilmsAsync(byte id)
            => await _context.FilmCategories.AnyAsync(fc => fc.CategoryId == id);
    }
}
