using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IFilmRepository
    {
        Task<IEnumerable<Film>> GetAllAsync();
        Task<Film?> GetByIdAsync(int id);
        Task<Film?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Film>> SearchByTitleAsync(string title);
        Task<IEnumerable<Film>> GetByRatingAsync(string rating);
        Task<IEnumerable<Film>> GetByCategoryAsync(byte categoryId);
        Task<IEnumerable<Film>> GetByActorAsync(int actorId);
        Task<IEnumerable<Film>> GetByLanguageAsync(byte languageId);
        Task<IEnumerable<Film>> GetByReleaseYearAsync(string year);
        Task<Film> CreateAsync(Film film);
        Task<Film> UpdateAsync(Film film);
        Task DeleteAsync(Film film);
        Task<bool> ExistsAsync(int id);
        Task UpdateRentalRateAsync(int id, decimal rate);
        Task<int> GetInventoryCountAsync(int id);
    }
}
