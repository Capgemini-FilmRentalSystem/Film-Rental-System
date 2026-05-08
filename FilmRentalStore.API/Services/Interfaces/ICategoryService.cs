using FilmRentalStore.API.DTOs.Category;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(byte id);
        Task<CategoryResponseDto> GetByNameAsync(string name);
        Task<IEnumerable<CategoryWithFilmCountDto>> GetCategoriesWithFilmCountAsync();
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryResponseDto> UpdateAsync(byte id, UpdateCategoryDto dto);
        Task DeleteAsync(byte id);
    }
}
