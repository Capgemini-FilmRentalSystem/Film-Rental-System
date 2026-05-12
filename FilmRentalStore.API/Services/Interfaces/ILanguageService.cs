using FilmRentalStore.API.DTOs.Language;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ILanguageService
    {
        /// <summary>Returns all languages; throws NotFoundException when no languages exist.</summary>
        Task<IEnumerable<LanguageResponseDto>> GetAllLanguagesAsync();

        Task<LanguageResponseDto> GetLanguageByIdAsync(byte languageId);

        Task<LanguageResponseDto> CreateLanguageAsync(LanguageRequestDto languageDto);

        Task<LanguageResponseDto> UpdateLanguageAsync(byte languageId, LanguageRequestDto languageDto);
    }
}
