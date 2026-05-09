using FilmRentalStore.API.DTOs.Actor;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IActorService
    {
        Task<IEnumerable<ActorResponseDto>> GetAllActorsAsync();

        Task<ActorResponseDto> GetActorByIdAsync(short id);

        Task<ActorResponseDto> CreateActorAsync(ActorDTO actorDto);

        Task<ActorResponseDto> UpdateActorAsync(short id, ActorDTO actorDto);
    }
}