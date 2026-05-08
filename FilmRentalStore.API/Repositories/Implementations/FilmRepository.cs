using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class FilmRepository : IFilmRepository
    {
        private readonly ApplicationDbContext _context;

        public FilmRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Film>> GetAllAsync()
            => await _context.Films
                .Include(f => f.Language)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Film?> GetByIdAsync(int id)
            => await _context.Films
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FilmId == id);

        public async Task<Film?> GetByIdWithDetailsAsync(int id)
            => await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Include(f => f.FilmActors)
                    .ThenInclude(fa => fa.Actor)
                .Include(f => f.FilmCategories)
                    .ThenInclude(fc => fc.Category)
                .Include(f => f.Inventories)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FilmId == id);

        public async Task<IEnumerable<Film>> SearchByTitleAsync(string title)
            => await _context.Films
                .Include(f => f.Language)
                .Where(f => f.Title.Contains(title))
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Film>> GetByRatingAsync(string rating)
            => await _context.Films
                .Include(f => f.Language)
                .Where(f => f.Rating == rating)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Film>> GetByCategoryAsync(byte categoryId)
            => await _context.Films
                .Include(f => f.Language)
                .Include(f => f.FilmCategories)
                    .ThenInclude(fc => fc.Category)
                .Where(f => f.FilmCategories.Any(fc => fc.CategoryId == categoryId))
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Film>> GetByActorAsync(int actorId)
            => await _context.Films
                .Include(f => f.Language)
                .Include(f => f.FilmActors)
                    .ThenInclude(fa => fa.Actor)
                .Where(f => f.FilmActors.Any(fa => fa.ActorId == actorId))
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Film>> GetByLanguageAsync(byte languageId)
            => await _context.Films
                .Include(f => f.Language)
                .Where(f => f.LanguageId == languageId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Film>> GetByReleaseYearAsync(string year)
            => await _context.Films
                .Include(f => f.Language)
                .Where(f => f.ReleaseYear == year)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Film> CreateAsync(Film film)
        {
            film.LastUpdate = DateTime.UtcNow;
            _context.Films.Add(film);
            await _context.SaveChangesAsync();
            return film;
        }

        public async Task<Film> UpdateAsync(Film film)
        {
            film.LastUpdate = DateTime.UtcNow;
            _context.Films.Update(film);
            await _context.SaveChangesAsync();
            return film;
        }

        public async Task DeleteAsync(Film film)
        {
            _context.Films.Remove(film);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Films.AnyAsync(f => f.FilmId == id);

        public async Task UpdateRentalRateAsync(int id, decimal rate)
        {
            await _context.Films
                .Where(f => f.FilmId == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(f => f.RentalRate, rate)
                    .SetProperty(f => f.LastUpdate, DateTime.UtcNow));
        }

        public async Task<int> GetInventoryCountAsync(int id)
            => await _context.Inventories.CountAsync(i => i.FilmId == id);
    }
}
