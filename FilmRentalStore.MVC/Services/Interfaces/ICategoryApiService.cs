using FilmRentalStore.MVC.DTOs.Category;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface ICategoryApiService
    {
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto?> GetCategoryByIdAsync(byte categoryId);
        Task<CategoryResponseDto?> CreateCategoryAsync(CategoryRequestDto dto);
        Task<CategoryResponseDto?> UpdateCategoryAsync(byte categoryId, CategoryRequestDto dto);
    }
}