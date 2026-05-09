using FilmRentalStore.API.DTOs.Category;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(byte categoryId)
        {
            var category = await _categoryService.GetByIdAsync(categoryId);

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            var createdCategory = await _categoryService.CreateAsync(categoryDto);

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { categoryId = createdCategory.CategoryId },
                createdCategory
            );
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(byte categoryId, [FromBody] CategoryDto categoryDto)
        {
            var updatedCategory = await _categoryService.UpdateAsync(categoryId, categoryDto);

            return Ok(updatedCategory);
        }
    }
}