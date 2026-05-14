using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class ActorApiService : IActorApiService
    {
        private readonly IApiClient _apiClient;

        public ActorApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
    }
}
