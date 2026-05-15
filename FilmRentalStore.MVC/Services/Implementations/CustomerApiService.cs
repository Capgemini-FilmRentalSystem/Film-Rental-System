using FilmRentalStore.MVC.DTOs.Customers;
using FilmRentalStore.MVC.Helpers;
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
            var endpoint = $"{ApiRoutes.Customers}?page={page}&pageSize={pageSize}";
            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<CustomerResponseDto> GetMeAsync()
        {
            var endpoint = $"{ApiRoutes.Customers}/me";
            return await _apiClient.GetAsync<CustomerResponseDto>(endpoint) ?? new CustomerResponseDto();
        }

        public async Task<CustomerResponseDto> GetByIdAsync(int id)
        {
            var endpoint = $"{ApiRoutes.Customers}/{id}";
            return await _apiClient.GetAsync<CustomerResponseDto>(endpoint) ?? new CustomerResponseDto();
        }

        public async Task<List<CustomerResponseDto>> SearchAsync(string? name, string? email)
        {
            var query = new List<string>();

            if (!string.IsNullOrEmpty(name))
            {
                query.Add($"name={Uri.EscapeDataString(name)}");
            }

            if (!string.IsNullOrEmpty(email))
            {
                query.Add($"email={Uri.EscapeDataString(email)}");
            }

            var endpoint = $"{ApiRoutes.Customers}/search";
            if (query.Count > 0)
            {
                endpoint += $"?{string.Join("&", query)}";
            }

            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<List<CustomerResponseDto>> GetActiveAsync()
        {
            var endpoint = $"{ApiRoutes.Customers}/active";
            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<List<CustomerResponseDto>> GetByStoreAsync(int storeId)
        {
            var endpoint = $"{ApiRoutes.Customers}/store/{storeId}";
            var response = await _apiClient.GetAsync<List<CustomerResponseDto>>(endpoint);
            return response ?? new List<CustomerResponseDto>();
        }

        public async Task<CustomerResponseDto> CreateAsync(CustomerRequestDto dto)
        {
            var endpoint = ApiRoutes.Customers;
            return await _apiClient.PostAsync<CustomerRequestDto, CustomerResponseDto>(endpoint, dto) ?? new CustomerResponseDto();
        }

        public async Task<CustomerResponseDto> UpdateAsync(int id, CustomerRequestDto dto)
        {
            var endpoint = $"{ApiRoutes.Customers}/{id}";
            return await _apiClient.PutAsync<CustomerRequestDto, CustomerResponseDto>(endpoint, dto) ?? new CustomerResponseDto();
        }

        public async Task ActivateAsync(int id)
        {
            var endpoint = $"{ApiRoutes.Customers}/{id}/activate";
            await _apiClient.PatchAsync<object, object>(endpoint, new { });
        }

        public async Task DeactivateAsync(int id)
        {
            var endpoint = $"{ApiRoutes.Customers}/{id}/deactivate";
            await _apiClient.PatchAsync<object, object>(endpoint, new { });
        }
    }
}

