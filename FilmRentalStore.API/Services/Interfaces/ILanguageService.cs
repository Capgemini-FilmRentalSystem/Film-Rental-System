using FilmRentalStore.API.DTOs.Language;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ILanguageService
    {
        Task<IEnumerable<LanguageResponseDto>> GetAllLanguagesAsync();

        Task<LanguageResponseDto> GetLanguageByIdAsync(byte languageId);

        Task<LanguageResponseDto> CreateLanguageAsync(LanguageDto languageDto);

        Task<LanguageResponseDto> UpdateLanguageAsync(byte languageId, LanguageDto languageDto);
    }
}