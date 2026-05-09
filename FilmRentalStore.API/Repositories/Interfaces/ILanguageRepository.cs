using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface ILanguageRepository
    {
        Task<IEnumerable<Language>> GetAllAsync();

        Task<Language?> GetByIdAsync(byte languageId);

        Task<bool> LanguageExistsAsync(byte languageId);

        Task<bool> LanguageNameExistsAsync(string name);

        Task AddAsync(Language language);

        void Update(Language language);

        Task<bool> SaveChangesAsync();
    }
}