using FilmRentalStore.MVC.ViewModels.Actor;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IActorApiService
    {
        Task<IEnumerable<ActorViewModel>> GetAllActorsAsync();

        Task<ActorViewModel?> GetActorByIdAsync(int id);

        Task CreateActorAsync(ActorViewModel model);

        Task UpdateActorAsync(int id, ActorViewModel model);
    }
}