using FilmRentalStore.API.DTOs.Category;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin,Manager,Staff,Customer")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [Authorize(Roles = "Admin,Manager,Staff,Customer")]
        public async Task<IActionResult> GetCategoryById(byte categoryId)
        {
            var category = await _categoryService.GetByIdAsync(categoryId);

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequestDto categoryDto)
        {
            var createdCategory = await _categoryService.CreateAsync(categoryDto);

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { categoryId = createdCategory.CategoryId },
                createdCategory
            );
        }

        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateCategory(byte categoryId, [FromBody] CategoryRequestDto categoryDto)
        {
            var updatedCategory = await _categoryService.UpdateAsync(categoryId, categoryDto);

            return Ok(updatedCategory);
        }
    }
}
