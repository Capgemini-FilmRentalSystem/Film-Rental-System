using FilmRentalStore.MVC.DTOs.Customers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class CustomerApiService : ICustomerApiService
    {
        private readonly IApiClient _apiClient;

        public CustomerApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<CustomerResponseDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var endpoint = $"api/customers?page={page}&pageSize={pageSize}";
            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<CustomerResponseDto> GetByIdAsync(int id)
        {
            var endpoint = $"api/customers/{id}";
            return await _apiClient.GetAsync<CustomerResponseDto>(endpoint) ?? new CustomerResponseDto();
        }

        public async Task<List<CustomerResponseDto>> SearchAsync(string? name, string? email)
        {
            var endpoint = "api/customers/search";
            if (!string.IsNullOrEmpty(name))
                endpoint += $"?name={Uri.EscapeDataString(name)}";
            if (!string.IsNullOrEmpty(email))
                endpoint += $"&email={Uri.EscapeDataString(email)}";

            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<List<CustomerResponseDto>> GetActiveAsync()
        {
            var endpoint = "api/customers/active";
            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<List<CustomerResponseDto>> GetByStoreAsync(int storeId)
        {
            var endpoint = $"api/customers/store/{storeId}";
            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<CustomerResponseDto> CreateAsync(CustomerRequestDto dto)
        {
            var endpoint = "api/customers";
            return await _apiClient.PostAsync<CustomerRequestDto, CustomerResponseDto>(endpoint, dto) ?? new CustomerResponseDto();
        }

        public async Task<CustomerResponseDto> UpdateAsync(int id, CustomerRequestDto dto)
        {
            var endpoint = $"api/customers/{id}";
            return await _apiClient.PutAsync<CustomerRequestDto, CustomerResponseDto>(endpoint, dto) ?? new CustomerResponseDto();
        }

        public async Task ActivateAsync(int id)
        {
            var endpoint = $"api/customers/{id}/activate";
            await _apiClient.PatchAsync<object, object>(endpoint, new { });
        }

        public async Task DeactivateAsync(int id)
        {
            var endpoint = $"api/customers/{id}/deactivate";
            await _apiClient.PatchAsync<object, object>(endpoint, new { });
        }
    }
}

