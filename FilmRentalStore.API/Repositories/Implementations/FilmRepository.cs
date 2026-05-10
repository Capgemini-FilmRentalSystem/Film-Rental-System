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
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .ToListAsync();
        }

        public async Task<Film?> GetByIdAsync(int filmId)
        {
            return await _context.Films
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
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

            _context.Films.Update(film);
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

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}