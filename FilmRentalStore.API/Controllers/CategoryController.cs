using FilmRentalStore.API.DTOs.Category;
using FilmRentalStore.API.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CreateCategoryDto> _createValidator;
        private readonly IValidator<UpdateCategoryDto> _updateValidator;

        public CategoryController(
            ICategoryService categoryService,
            IValidator<CreateCategoryDto> createValidator,
            IValidator<UpdateCategoryDto> updateValidator)
        {
            _categoryService = categoryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // GET api/category
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        // GET api/category/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(byte id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(result);
        }

        // GET api/category/name/{name}
        [HttpGet("name/{name}")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _categoryService.GetByNameAsync(name);
            return Ok(result);
        }

        // GET api/category/film-count
        [HttpGet("film-count")]
        [ProducesResponseType(typeof(IEnumerable<CategoryWithFilmCountDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWithFilmCount()
        {
            var result = await _categoryService.GetCategoriesWithFilmCountAsync();
            return Ok(result);
        }

        // POST api/category
        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
        }

        // PUT api/category/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(byte id, [FromBody] UpdateCategoryDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return UnprocessableEntity(validation.Errors.Select(e => e.ErrorMessage));

            var updated = await _categoryService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        //// DELETE api/category/{id}
        //[HttpDelete("{id:int}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        //public async Task<IActionResult> Delete(byte id)
        //{
        //    await _categoryService.DeleteAsync(id);
        //    return NoContent();
        //}
    }
}
