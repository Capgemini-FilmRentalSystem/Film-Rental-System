using FilmRentalStore.API.DTOs.Category;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ICategoryService
    {
        /// <summary>Returns all categories; throws NotFoundException when no categories exist.</summary>
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(byte id);
        Task<CategoryResponseDto> CreateAsync(CategoryRequestDto dto);
        Task<CategoryResponseDto> UpdateAsync(byte id, CategoryRequestDto dto);
    }
}
