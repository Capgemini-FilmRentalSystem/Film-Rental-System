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

        public async Task<Category> AddAsync(Category category)
        {
            category.LastUpdate = DateTime.UtcNow;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public void Update(Category category)
        {
            category.LastUpdate = DateTime.UtcNow;
            _context.Categories.Update(category);
        }

        public async Task<bool> ExistsAsync(byte id)
            => await _context.Categories.AnyAsync(c => c.CategoryId == id);

        public async Task<bool> NameExistsAsync(string name)
            => await _context.Categories.AnyAsync(c => c.Name == name);

        public async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}
