using FilmRentalStore.MVC.DTOs.Language;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface ILanguageApiService
    {
        Task<List<LanguageResponseDto>> GetAllAsync();
        Task<LanguageResponseDto> GetByIdAsync(byte id);
        Task<LanguageResponseDto> CreateAsync(LanguageRequestDto dto);
        Task<LanguageResponseDto> UpdateAsync(byte id, LanguageRequestDto dto);
    }
}
