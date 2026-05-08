using FilmRentalStore.API.DTOs.Actor;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IActorService
    {
        Task<IEnumerable<ActorDTO>> GetAllActorsAsync();

        Task<ActorDTO?> GetActorByIdAsync(short id);

        Task<ActorDTO> CreateActorAsync(ActorDTO actorDto);

        Task<bool> UpdateActorAsync(short id, ActorDTO actorDto);

        Task<bool> DeleteActorAsync(short id);
    }
}
