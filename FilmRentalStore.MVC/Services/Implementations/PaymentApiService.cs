using FilmRentalStore.MVC.DTOs.Payment;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class PaymentApiService : IPaymentApiService
    {
        private readonly IApiClient _apiClient;

        public PaymentApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<PaymentResponseDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var endpoint = $"{ApiRoutes.Payments}?page={page}&pageSize={pageSize}";
            return await GetListAsync(endpoint, "No payments found");
        }

        public async Task<List<PaymentResponseDto>> GetMineAsync(int page = 1, int pageSize = 10)
        {
            var endpoint = $"{ApiRoutes.Payments}/me?page={page}&pageSize={pageSize}";
            return await GetListAsync(endpoint, "No payments found");
        }

        public async Task<PaymentResponseDto?> GetByIdAsync(int id)
        {
            return await _apiClient.GetAsync<PaymentResponseDto>($"{ApiRoutes.Payments}/{id}");
        }

        public async Task<PaymentResponseDto?> GetMineByIdAsync(int id)
        {
            return await _apiClient.GetAsync<PaymentResponseDto>($"{ApiRoutes.Payments}/me/{id}");
        }

        public async Task<PaymentResponseDto?> CreateAsync(PaymentRequestDto dto)
        {
            return await _apiClient.PostAsync<PaymentRequestDto, PaymentResponseDto>(ApiRoutes.Payments, dto);
        }

        private async Task<List<PaymentResponseDto>> GetListAsync(string endpoint, string emptyMessage)
        {
            try
            {
                return await _apiClient.GetAsync<List<PaymentResponseDto>>(endpoint) ?? new();
            }
            catch (Exception ex) when (ex.Message.Contains(emptyMessage, StringComparison.OrdinalIgnoreCase))
            {
                return new();
            }
        }
    }
}
