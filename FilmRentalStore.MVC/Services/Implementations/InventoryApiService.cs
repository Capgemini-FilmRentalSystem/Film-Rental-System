using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class InventoryApiService : IInventoryApiService
    {
        private readonly IApiClient _apiClient;

        public InventoryApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}
