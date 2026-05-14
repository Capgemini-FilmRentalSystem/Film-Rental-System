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
    }
}
