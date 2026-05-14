using FilmRentalStore.MVC.DTOs.Language;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class LanguageApiService : ILanguageApiService
    {
        private readonly IApiClient _apiClient;

        public LanguageApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<LanguageResponseDto>> GetAllAsync()
        {
            var endpoint = "api/languages";
            var response = await _apiClient.GetAsync<List<LanguageResponseDto>>(endpoint);
            return response ?? new List<LanguageResponseDto>();
        }

        public async Task<LanguageResponseDto> GetByIdAsync(byte id)
        {
            var endpoint = $"api/languages/{id}";
            return await _apiClient.GetAsync<LanguageResponseDto>(endpoint) ?? new LanguageResponseDto();
        }

        public async Task<LanguageResponseDto> CreateAsync(LanguageRequestDto dto)
        {
            var endpoint = "api/languages";
            return await _apiClient.PostAsync<LanguageRequestDto, LanguageResponseDto>(endpoint, dto) ?? new LanguageResponseDto();
        }

        public async Task<LanguageResponseDto> UpdateAsync(byte id, LanguageRequestDto dto)
        {
            var endpoint = $"api/languages/{id}";
            return await _apiClient.PutAsync<LanguageRequestDto, LanguageResponseDto>(endpoint, dto) ?? new LanguageResponseDto();
        }
    }
}

