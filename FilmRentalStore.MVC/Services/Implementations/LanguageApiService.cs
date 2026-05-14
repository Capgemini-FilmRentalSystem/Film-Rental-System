using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class LanguageApiService : ILanguageApiService
    {
        private readonly IApiClient _apiClient;

        public LanguageApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}
