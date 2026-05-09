using FilmRentalStore.API.DTOs.Film;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IFilmService
    {
        Task<IEnumerable<FilmResponseDto>> GetAllFilmsAsync();

        Task<FilmResponseDto> GetFilmByIdAsync(int filmId);

        Task<FilmResponseDto> CreateFilmAsync(FilmDto filmDto);

        Task<FilmResponseDto> UpdateFilmAsync(int filmId, FilmDto filmDto);

        Task AssignActorToFilmAsync(int filmId, FilmActorAssignDto dto);

        Task RemoveActorFromFilmAsync(int filmId, short actorId);

        Task AssignCategoryToFilmAsync(int filmId, FilmCategoryAssignDto dto);

        Task RemoveCategoryFromFilmAsync(int filmId, byte categoryId);
    }
}