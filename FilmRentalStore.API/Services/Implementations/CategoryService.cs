using FilmRentalStore.API.DTOs.Category;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;
using static FilmRentalStore.API.Exceptions.Exceptions;

namespace FilmRentalStore.API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return categories.Select(MapToResponse);
        }

        public async Task<CategoryResponseDto> GetByIdAsync(byte id)
        {
            var category = await _categoryRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Category", id);
            return MapToResponse(category);
        }

        public async Task<CategoryResponseDto> GetByNameAsync(string name)
        {
            var category = await _categoryRepo.GetByNameAsync(name)
                ?? throw new NotFoundException($"Category with name '{name}' was not found.");
            return MapToResponse(category);
        }

        public async Task<IEnumerable<CategoryWithFilmCountDto>> GetCategoriesWithFilmCountAsync()
        {
            var categories = await _categoryRepo.GetCategoriesWithFilmCountAsync();
            return categories.Select(c => new CategoryWithFilmCountDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                LastUpdate = c.LastUpdate,
                FilmCount = c.FilmCategories?.Count ?? 0
            });
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
        {
            if (await _categoryRepo.NameExistsAsync(dto.Name))
                throw new ConflictException($"Category '{dto.Name}' already exists.");

            var category = new Category
            {
                Name = dto.Name,
                LastUpdate = DateTime.UtcNow
            };

            var created = await _categoryRepo.CreateAsync(category);
            return MapToResponse(created);
        }

        public async Task<CategoryResponseDto> UpdateAsync(byte id, UpdateCategoryDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Category", id);

            if (await _categoryRepo.NameExistsAsync(dto.Name, id))
                throw new ConflictException($"Category name '{dto.Name}' is already taken.");

            category.Name = dto.Name;

            var updated = await _categoryRepo.UpdateAsync(category);
            return MapToResponse(updated);
        }

        public async Task DeleteAsync(byte id)
        {
            var category = await _categoryRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Category", id);

            if (await _categoryRepo.HasFilmsAsync(id))
                throw new ConflictException(
                    $"Cannot delete category '{category.Name}': it has films assigned. Reassign them first.");

            await _categoryRepo.DeleteAsync(category);
        }

        private static CategoryResponseDto MapToResponse(Category c) => new()
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            LastUpdate = c.LastUpdate
        };
    }
}
