using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly IApiClient _apiClient;

        public CategoryApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}
