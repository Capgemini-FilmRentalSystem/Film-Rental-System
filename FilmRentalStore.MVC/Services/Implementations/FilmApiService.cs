using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class FilmApiService : IFilmApiService
    {
        private readonly IApiClient _apiClient;

        public FilmApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}
