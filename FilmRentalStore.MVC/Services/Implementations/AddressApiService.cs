using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class AddressApiService : IAddressApiService
    {
        private readonly IApiClient _apiClient;

        public AddressApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<AddressResponseDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var endpoint = $"api/addresses?page={page}&pageSize={pageSize}";
            var response = await _apiClient.GetAsync<List<AddressResponseDto>>(endpoint);
            return response ?? new List<AddressResponseDto>();
        }

        public async Task<AddressResponseDto> GetByIdAsync(int id)
        {
            var endpoint = $"api/addresses/{id}";
            return await _apiClient.GetAsync<AddressResponseDto>(endpoint) ?? new AddressResponseDto();
        }

        public async Task<AddressResponseDto> CreateAsync(AddressRequestDto dto)
        {
            var endpoint = "api/addresses";
            return await _apiClient.PostAsync<AddressRequestDto, AddressResponseDto>(endpoint, dto) ?? new AddressResponseDto();
        }

        public async Task<AddressResponseDto> UpdateAsync(int id, AddressRequestDto dto)
        {
            var endpoint = $"api/addresses/{id}";
            return await _apiClient.PutAsync<AddressRequestDto, AddressResponseDto>(endpoint, dto) ?? new AddressResponseDto();
        }

        public async Task DeleteAsync(int id)
        {
            var endpoint = $"api/addresses/{id}";
            await _apiClient.DeleteAsync(endpoint);
        }
    }
}
