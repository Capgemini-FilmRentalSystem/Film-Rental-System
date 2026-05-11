using FilmRentalStore.API.DTOs.Actor;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IActorService
    {
        /// <summary>Returns all actors; throws NotFoundException when no actors exist.</summary>
        Task<IEnumerable<ActorResponseDto>> GetAllActorsAsync();

        Task<ActorResponseDto> GetActorByIdAsync(int id);

        Task<ActorResponseDto> CreateActorAsync(ActorDTO actorDto);

        Task<ActorResponseDto> UpdateActorAsync(int id, ActorDTO actorDto);
    }
}
