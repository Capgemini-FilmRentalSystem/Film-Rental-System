using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IActorRepository
    {
        Task<IEnumerable<Actor>> GetAllActorsAsync();

        Task<Actor?> GetActorByIdAsync(short id);

        Task<Actor> CreateActorAsync(Actor actor);

        Task<bool> UpdateActorAsync(Actor actor);

        Task<bool> DeleteActorAsync(short id);
    }
}
