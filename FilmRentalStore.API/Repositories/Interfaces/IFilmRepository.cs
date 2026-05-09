using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IFilmRepository
    {
        Task<IEnumerable<Film>> GetAllAsync();

        Task<Film?> GetByIdAsync(int filmId);

        Task<bool> FilmExistsAsync(int filmId);

        Task AddAsync(Film film);

        void Update(Film film);

        Task<bool> IsActorAssignedAsync(int filmId, short actorId);

        Task<bool> IsCategoryAssignedAsync(int filmId, byte categoryId);

        Task AssignActorAsync(int filmId, short actorId);

        Task RemoveActorAsync(int filmId, short actorId);

        Task AssignCategoryAsync(int filmId, byte categoryId);

        Task RemoveCategoryAsync(int filmId, byte categoryId);

        Task<bool> SaveChangesAsync();
    }
}