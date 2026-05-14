using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Payment;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class PaymentApiService : IPaymentApiService
    {
        private readonly IApiClient _apiClient;

        public PaymentApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<PaymentViewModel>> GetAllPaymentsAsync(
            int page,
            int pageSize)
        {
            return await _apiClient.GetAsync<IEnumerable<PaymentViewModel>>(
                $"api/Payments?page={page}&pageSize={pageSize}")
                ?? new List<PaymentViewModel>();
        }

        public async Task<IEnumerable<PaymentViewModel>> GetMyPaymentsAsync(
            int page,
            int pageSize)
        {
            return await _apiClient.GetAsync<IEnumerable<PaymentViewModel>>(
                $"api/Payments/me?page={page}&pageSize={pageSize}")
                ?? new List<PaymentViewModel>();
        }

        public async Task<PaymentViewModel?> GetPaymentByIdAsync(int paymentId)
        {
            return await _apiClient.GetAsync<PaymentViewModel>(
                $"api/Payments/{paymentId}");
        }

        public async Task CreatePaymentAsync(PaymentViewModel model)
        {
            await _apiClient.PostAsync<PaymentViewModel, PaymentViewModel>(
                "api/Payments",
                model);
        }
    }
}