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
    }
}
