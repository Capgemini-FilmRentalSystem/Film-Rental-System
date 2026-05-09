using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IActorRepository
    {
        Task<IEnumerable<Actor>> GetAllActorsAsync();
        Task<Actor?> GetActorByIdAsync(short id);
        Task<bool> ActorExistsAsync(int actorId);
        Task CreateActorAsync(Actor actor);
        void Update(Actor actor);
        Task<bool> SaveChangesAsync();
    }
}
