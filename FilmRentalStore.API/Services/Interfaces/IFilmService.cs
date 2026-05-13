using FilmRentalStore.API.DTOs.Film;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IFilmService
    {
        const int DefaultPage = 1;
        const int DefaultPageSize = 10;
        const int MaxPageSize = 100;

        /// <summary>Returns a paged film result; page and pageSize must be within the defined pagination bounds.</summary>
        Task<IEnumerable<FilmResponseDto>> GetAllFilmsAsync(int page, int pageSize);

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
