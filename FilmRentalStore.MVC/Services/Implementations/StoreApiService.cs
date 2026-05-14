using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class StoreApiService : IStoreApiService
    {
        private readonly IApiClient _apiClient;

        public StoreApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}
