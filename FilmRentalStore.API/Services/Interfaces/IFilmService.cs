using FilmRentalStore.API.DTOs.Film;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IFilmService
    {
        /// <summary>Returns all films; throws NotFoundException when no films exist.</summary>
        Task<IEnumerable<FilmResponseDto>> GetAllFilmsAsync();

        Task<FilmResponseDto> GetFilmByIdAsync(int filmId);

        Task<FilmResponseDto> CreateFilmAsync(FilmRequestDto filmDto);

        Task<FilmResponseDto> UpdateFilmAsync(int filmId, FilmRequestDto filmDto);

        Task AssignActorToFilmAsync(int filmId, FilmActorAssignRequestDto dto);

        Task RemoveActorFromFilmAsync(int filmId, int actorId);

        Task AssignCategoryToFilmAsync(int filmId, FilmCategoryAssignRequestDto dto);

        Task RemoveCategoryFromFilmAsync(int filmId, byte categoryId);
    }
}
