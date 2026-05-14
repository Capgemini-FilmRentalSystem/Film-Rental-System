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
    }
}
