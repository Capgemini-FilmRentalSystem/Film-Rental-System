using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class AuthApiService : IAuthApiService
    {
        private readonly IApiClient _apiClient;

        public AuthApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}
