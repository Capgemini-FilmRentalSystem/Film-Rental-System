using FilmRentalStore.API.DTOs.Film;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly IFilmService _filmService;

        public FilmsController(IFilmService filmService)
        {
            _filmService = filmService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFilms()
        {
            var films = await _filmService.GetAllFilmsAsync();

            return Ok(films);
        }

        [HttpGet("{filmId}")]
        public async Task<IActionResult> GetFilmById(int filmId)
        {
            var film = await _filmService.GetFilmByIdAsync(filmId);

            return Ok(film);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFilm([FromBody] FilmRequestDto filmDto)
        {
            var createdFilm = await _filmService.CreateFilmAsync(filmDto);

            return CreatedAtAction(
                nameof(GetFilmById),
                new { filmId = createdFilm.FilmId },
                createdFilm
            );
        }

        [HttpPut("{filmId}")]
        public async Task<IActionResult> UpdateFilm(int filmId, [FromBody] FilmRequestDto filmDto)
        {
            var updatedFilm = await _filmService.UpdateFilmAsync(filmId, filmDto);

            return Ok(updatedFilm);
        }

        [HttpPost("{filmId}/actors")]
        public async Task<IActionResult> AssignActorToFilm(
            int filmId,
            [FromBody] FilmActorAssignRequestDto dto)
        {
            await _filmService.AssignActorToFilmAsync(filmId, dto);

            return NoContent();
        }

        [HttpDelete("{filmId}/actors/{actorId}")]
        public async Task<IActionResult> RemoveActorFromFilm(int filmId, int actorId)
        {
            await _filmService.RemoveActorFromFilmAsync(filmId, actorId);

            return NoContent();
        }

        [HttpPost("{filmId}/categories")]
        public async Task<IActionResult> AssignCategoryToFilm(
            int filmId,
            [FromBody] FilmCategoryAssignRequestDto dto)
        {
            await _filmService.AssignCategoryToFilmAsync(filmId, dto);

            return NoContent();
        }

        [HttpDelete("{filmId}/categories/{categoryId}")]
        public async Task<IActionResult> RemoveCategoryFromFilm(int filmId, byte categoryId)
        {
            await _filmService.RemoveCategoryFromFilmAsync(filmId, categoryId);

            return NoContent();
        }
    }
}