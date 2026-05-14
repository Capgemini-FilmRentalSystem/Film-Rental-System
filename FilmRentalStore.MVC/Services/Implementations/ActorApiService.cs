using FilmRentalStore.MVC.Services.Interfaces;
using FilmRentalStore.MVC.ViewModels.Actor;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class ActorApiService : IActorApiService
    {
        private readonly IApiClient _apiClient;

        public ActorApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<ActorViewModel>> GetAllActorsAsync()
        {
            return await _apiClient.GetAsync<IEnumerable<ActorViewModel>>(
                "api/Actors")
                ?? new List<ActorViewModel>();
        }

        public async Task<ActorViewModel?> GetActorByIdAsync(int id)
        {
            return await _apiClient.GetAsync<ActorViewModel>(
                $"api/Actors/{id}");
        }

        public async Task CreateActorAsync(ActorViewModel model)
        {
            await _apiClient.PostAsync<ActorViewModel, ActorViewModel>(
                "api/Actors",
                model);
        }

        public async Task UpdateActorAsync(int id, ActorViewModel model)
        {
            await _apiClient.PutAsync<ActorViewModel, ActorViewModel>(
                $"api/Actors/{id}",
                model);
        }
    }
}