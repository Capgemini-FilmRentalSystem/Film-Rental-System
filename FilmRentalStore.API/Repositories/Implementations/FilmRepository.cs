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
        {
            return await _context.Films
                .AsNoTracking()
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Include(f => f.FilmCategories)
                    .ThenInclude(fc => fc.Category)
                .Include(f => f.FilmActors)
                    .ThenInclude(fa => fa.Actor)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Film> Films, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var query = _context.Films
                .AsNoTracking()
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Include(f => f.FilmCategories)
                    .ThenInclude(fc => fc.Category)
                .Include(f => f.FilmActors)
                    .ThenInclude(fa => fa.Actor);

            var totalCount = await query.CountAsync();

            var films = await query
                .OrderBy(f => f.FilmId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (films, totalCount);
        }

        public async Task<Film?> GetByIdAsync(int filmId)
        {
            return await _context.Films
                .AsNoTracking()
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Include(f => f.FilmCategories)
                    .ThenInclude(fc => fc.Category)
                .Include(f => f.FilmActors)
                    .ThenInclude(fa => fa.Actor)
                .FirstOrDefaultAsync(f => f.FilmId == filmId);
        }

        public async Task<Film?> GetEntityByIdAsync(int filmId)
        {
            return await _context.Films
                .FirstOrDefaultAsync(f => f.FilmId == filmId);
        }

        public async Task<bool> FilmExistsAsync(int filmId)
        {
            return await _context.Films
                .AnyAsync(f => f.FilmId == filmId);
        }

        public async Task AddAsync(Film film)
        {
            film.LastUpdate = DateTime.Now;
            await _context.Films.AddAsync(film);
        }

        public void Update(Film film)
        {
            film.LastUpdate = DateTime.Now;
            _context.Entry(film).State = EntityState.Modified;
        }

        public async Task<bool> IsActorAssignedAsync(int filmId, int actorId)
        {
            return await _context.FilmActors
                .AnyAsync(fa => fa.FilmId == filmId && fa.ActorId == actorId);
        }

        public async Task<bool> IsCategoryAssignedAsync(int filmId, byte categoryId)
        {
            return await _context.FilmCategories
                .AnyAsync(fc => fc.FilmId == filmId && fc.CategoryId == categoryId);
        }

        public async Task AssignActorAsync(int filmId, int actorId)
        {
            var filmActor = new FilmActor
            {
                FilmId = filmId,
                ActorId = actorId,
                LastUpdate = DateTime.Now
            };

            await _context.FilmActors.AddAsync(filmActor);
        }

        public async Task RemoveActorAsync(int filmId, int actorId)
        {
            var filmActor = await _context.FilmActors
                .FirstOrDefaultAsync(fa => fa.FilmId == filmId && fa.ActorId == actorId);

            if (filmActor != null)
            {
                _context.FilmActors.Remove(filmActor);
            }
        }

        public async Task AssignCategoryAsync(int filmId, byte categoryId)
        {
            var filmCategory = new FilmCategory
            {
                FilmId = filmId,
                CategoryId = categoryId,
                LastUpdate = DateTime.Now
            };

            await _context.FilmCategories.AddAsync(filmCategory);
        }

        public async Task RemoveCategoryAsync(int filmId, byte categoryId)
        {
            var filmCategory = await _context.FilmCategories
                .FirstOrDefaultAsync(fc => fc.FilmId == filmId && fc.CategoryId == categoryId);

            if (filmCategory != null)
            {
                _context.FilmCategories.Remove(filmCategory);
            }
        }

        public async Task ReplaceActorsAsync(int filmId, IEnumerable<int> actorIds)
        {
            var selectedActorIds = actorIds.Distinct().ToHashSet();
            var existing = await _context.FilmActors
                .Where(fa => fa.FilmId == filmId)
                .ToListAsync();

            _context.FilmActors.RemoveRange(existing.Where(fa => !selectedActorIds.Contains(fa.ActorId)));

            var existingActorIds = existing.Select(fa => fa.ActorId).ToHashSet();
            foreach (var actorId in selectedActorIds.Where(actorId => !existingActorIds.Contains(actorId)))
            {
                await _context.FilmActors.AddAsync(new FilmActor
                {
                    FilmId = filmId,
                    ActorId = actorId,
                    LastUpdate = DateTime.Now
                });
            }
        }

        public async Task ReplaceCategoriesAsync(int filmId, IEnumerable<byte> categoryIds)
        {
            var selectedCategoryIds = categoryIds.Distinct().ToHashSet();
            var existing = await _context.FilmCategories
                .Where(fc => fc.FilmId == filmId)
                .ToListAsync();

            _context.FilmCategories.RemoveRange(existing.Where(fc => !selectedCategoryIds.Contains(fc.CategoryId)));

            var existingCategoryIds = existing.Select(fc => fc.CategoryId).ToHashSet();
            foreach (var categoryId in selectedCategoryIds.Where(categoryId => !existingCategoryIds.Contains(categoryId)))
            {
                await _context.FilmCategories.AddAsync(new FilmCategory
                {
                    FilmId = filmId,
                    CategoryId = categoryId,
                    LastUpdate = DateTime.Now
                });
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
