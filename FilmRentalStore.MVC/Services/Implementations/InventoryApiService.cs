using FilmRentalStore.MVC.DTOs.Inventory;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class InventoryApiService : IInventoryApiService
    {
        private readonly IApiClient _apiClient;

        public InventoryApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<InventoryResponseDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var endpoint = $"{ApiRoutes.Inventory}?page={page}&pageSize={pageSize}";
            return await _apiClient.GetAsync<List<InventoryResponseDto>>(endpoint) ?? new();
        }

        public async Task<InventoryResponseDto?> GetByIdAsync(int id)
        {
            return await _apiClient.GetAsync<InventoryResponseDto>($"{ApiRoutes.Inventory}/{id}");
        }

        public async Task<InventoryResponseDto?> CreateAsync(InventoryRequestDto dto)
        {
            return await _apiClient.PostAsync<InventoryRequestDto, InventoryResponseDto>(ApiRoutes.Inventory, dto);
        }

        public async Task<InventoryResponseDto?> UpdateAsync(int id, InventoryRequestDto dto)
        {
            return await _apiClient.PutAsync<InventoryRequestDto, InventoryResponseDto>($"{ApiRoutes.Inventory}/{id}", dto);
        }
    }
}
