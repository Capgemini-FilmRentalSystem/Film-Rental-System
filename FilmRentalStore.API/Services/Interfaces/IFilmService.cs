using FilmRentalStore.API.DTOs.Film;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IFilmService
    {
        Task<IEnumerable<FilmResponseDto>> GetAllAsync();
        Task<FilmDetailResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<FilmResponseDto>> SearchByTitleAsync(string title);
        Task<IEnumerable<FilmResponseDto>> GetByRatingAsync(string rating);
        Task<IEnumerable<FilmResponseDto>> GetByCategoryAsync(byte categoryId);
        Task<IEnumerable<FilmResponseDto>> GetByActorAsync(int actorId);
        Task<IEnumerable<FilmResponseDto>> GetByLanguageAsync(byte languageId);
        Task<IEnumerable<FilmResponseDto>> GetByReleaseYearAsync(string year);
        Task<FilmResponseDto> CreateAsync(CreateFilmDto dto);
        Task<FilmResponseDto> UpdateAsync(int id, UpdateFilmDto dto);
        Task DeleteAsync(int id);
        Task UpdateRentalRateAsync(int id, UpdateFilmRateDto dto);
    }
}
