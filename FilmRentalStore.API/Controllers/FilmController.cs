using FilmRentalStore.API.DTOs.Film;
using FilmRentalStore.API.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FilmController : ControllerBase
    {
        private readonly IFilmService _filmService;
        private readonly IValidator<CreateFilmDto> _createValidator;
        private readonly IValidator<UpdateFilmDto> _updateValidator;
        private readonly IValidator<UpdateFilmRateDto> _rateValidator;

        public FilmController(
            IFilmService filmService,
            IValidator<CreateFilmDto> createValidator,
            IValidator<UpdateFilmDto> updateValidator,
            IValidator<UpdateFilmRateDto> rateValidator)
        {
            _filmService = filmService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _rateValidator = rateValidator;
        }

        // GET api/film
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FilmResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _filmService.GetAllAsync();
            return Ok(result);
        }

        // GET api/film/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(FilmDetailResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _filmService.GetByIdAsync(id);
            return Ok(result);
        }

        // GET api/film/search?title=abc
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<FilmResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchByTitle([FromQuery] string title)
        {
            var result = await _filmService.SearchByTitleAsync(title);
            return Ok(result);
        }

        // GET api/film/rating/{rating}
        [HttpGet("rating/{rating}")]
        [ProducesResponseType(typeof(IEnumerable<FilmResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByRating(string rating)
        {
            var result = await _filmService.GetByRatingAsync(rating);
            return Ok(result);
        }

        // GET api/film/category/{categoryId}
        [HttpGet("category/{categoryId:int}")]
        [ProducesResponseType(typeof(IEnumerable<FilmResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategory(byte categoryId)
        {
            var result = await _filmService.GetByCategoryAsync(categoryId);
            return Ok(result);
        }

        // GET api/film/actor/{actorId}
        [HttpGet("actor/{actorId:int}")]
        [ProducesResponseType(typeof(IEnumerable<FilmResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByActor(int actorId)
        {
            var result = await _filmService.GetByActorAsync(actorId);
            return Ok(result);
        }

        // GET api/film/language/{languageId}
        [HttpGet("language/{languageId:int}")]
        [ProducesResponseType(typeof(IEnumerable<FilmResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLanguage(byte languageId)
        {
            var result = await _filmService.GetByLanguageAsync(languageId);
            return Ok(result);
        }

        // GET api/film/release-year/{year}
        [HttpGet("release-year/{year}")]
        [ProducesResponseType(typeof(IEnumerable<FilmResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByReleaseYear(string year)
        {
            var result = await _filmService.GetByReleaseYearAsync(year);
            return Ok(result);
        }

        // POST api/film
        [HttpPost]
        [ProducesResponseType(typeof(FilmResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody] CreateFilmDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var created = await _filmService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.FilmId }, created);
        }

        // PUT api/film/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(FilmResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFilmDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var updated = await _filmService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // DELETE api/film/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            await _filmService.DeleteAsync(id);
            return NoContent();
        }

        // PATCH api/film/{id}/rate
        [HttpPatch("{id:int}/rate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateRentalRate(int id, [FromBody] UpdateFilmRateDto dto)
        {
            var validation = await _rateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            await _filmService.UpdateRentalRateAsync(id, dto);
            return NoContent();
        }
    }
}
