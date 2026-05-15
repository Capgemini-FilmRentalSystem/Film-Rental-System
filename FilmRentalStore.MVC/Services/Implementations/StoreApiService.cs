using FilmRentalStore.MVC.DTOs.Store;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class StoreApiService : IStoreApiService
    {
        private readonly IApiClient _apiClient;

        public StoreApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<StoreResponseDto>> GetAllAsync()
        {
            return await _apiClient.GetAsync<List<StoreResponseDto>>(ApiRoutes.Stores) ?? new();
        }

        public async Task<StoreResponseDto?> GetByIdAsync(int id)
        {
            return await _apiClient.GetAsync<StoreResponseDto>($"{ApiRoutes.Stores}/{id}");
        }

        public async Task<StoreResponseDto?> CreateAsync(StoreRequestDto dto)
        {
            return await _apiClient.PostAsync<StoreRequestDto, StoreResponseDto>(ApiRoutes.Stores, dto);
        }

        public async Task<StoreResponseDto?> UpdateAsync(int id, StoreRequestDto dto)
        {
            return await _apiClient.PutAsync<StoreRequestDto, StoreResponseDto>($"{ApiRoutes.Stores}/{id}", dto);
        }
    }
}
