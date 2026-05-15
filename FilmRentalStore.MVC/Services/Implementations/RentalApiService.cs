using FilmRentalStore.MVC.DTOs.Rental;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class RentalApiService : IRentalApiService
    {
        private readonly IApiClient _apiClient;

        public RentalApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<RentalResponseDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var endpoint = $"{ApiRoutes.Rentals}?page={page}&pageSize={pageSize}";
            return await GetListAsync(endpoint, "No rentals found");
        }

        public async Task<List<RentalResponseDto>> GetMineAsync(int page = 1, int pageSize = 10)
        {
            var endpoint = $"{ApiRoutes.Rentals}/me?page={page}&pageSize={pageSize}";
            return await GetListAsync(endpoint, "No rentals found");
        }

        public async Task<RentalResponseDto?> GetByIdAsync(int id)
        {
            return await _apiClient.GetAsync<RentalResponseDto>($"{ApiRoutes.Rentals}/{id}");
        }

        public async Task<RentalResponseDto?> GetMineByIdAsync(int id)
        {
            return await _apiClient.GetAsync<RentalResponseDto>($"{ApiRoutes.Rentals}/me/{id}");
        }

        public async Task<RentalResponseDto?> CreateAsync(RentalRequestDto dto)
        {
            return await _apiClient.PostAsync<RentalRequestDto, RentalResponseDto>(ApiRoutes.Rentals, dto);
        }

        public async Task<RentalResponseDto?> ReturnAsync(int id, RentalReturnRequestDto dto)
        {
            return await _apiClient.PutAsync<RentalReturnRequestDto, RentalResponseDto>($"{ApiRoutes.Rentals}/{id}/return", dto);
        }

        private async Task<List<RentalResponseDto>> GetListAsync(string endpoint, string emptyMessage)
        {
            try
            {
                return await _apiClient.GetAsync<List<RentalResponseDto>>(endpoint) ?? new();
            }
            catch (Exception ex) when (ex.Message.Contains(emptyMessage, StringComparison.OrdinalIgnoreCase))
            {
                return new();
            }
        }
    }
}
