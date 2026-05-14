using FilmRentalStore.MVC.DTOs.Actor;
using FilmRentalStore.MVC.DTOs.Category;
using FilmRentalStore.MVC.DTOs.Film;
using FilmRentalStore.MVC.DTOs.Language;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IFilmApiService
    {
        Task<List<FilmResponseDto>> GetAllFilmsAsync(int page, int pageSize);
        Task<FilmResponseDto?> GetFilmByIdAsync(int filmId);
        Task<FilmResponseDto?> CreateFilmAsync(FilmRequestDto dto);
        Task<FilmResponseDto?> UpdateFilmAsync(int filmId, FilmRequestDto dto);
        Task AssignActorAsync(int filmId, FilmActorAssignRequestDto dto);
        Task RemoveActorAsync(int filmId, int actorId);
        Task AssignCategoryAsync(int filmId, FilmCategoryAssignRequestDto dto);
        Task RemoveCategoryAsync(int filmId, byte categoryId);
        Task<List<LanguageResponseDto>> GetLanguagesAsync();
        Task<List<ActorResponseDto>> GetActorsAsync();
        Task<List<CategoryResponseDto>> GetCategoriesAsync();
    }
}