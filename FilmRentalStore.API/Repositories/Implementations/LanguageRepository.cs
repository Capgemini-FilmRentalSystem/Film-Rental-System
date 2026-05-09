using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly ApplicationDbContext _context;

        public LanguageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Language>> GetAllAsync()
        {
            return await _context.Languages
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Language?> GetByIdAsync(byte languageId)
        {
            return await _context.Languages
                .FirstOrDefaultAsync(l => l.LanguageId == languageId);
        }

        public async Task<bool> LanguageExistsAsync(byte languageId)
        {
            return await _context.Languages
                .AnyAsync(l => l.LanguageId == languageId);
        }

        public async Task<bool> LanguageNameExistsAsync(string name)
        {
            return await _context.Languages
                .AnyAsync(l => l.Name.ToLower() == name.ToLower());
        }

        public async Task AddAsync(Language language)
        {
            language.LastUpdate = DateTime.Now;

            await _context.Languages.AddAsync(language);
        }

        public void Update(Language language)
        {
            language.LastUpdate = DateTime.Now;

            _context.Languages.Update(language);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}