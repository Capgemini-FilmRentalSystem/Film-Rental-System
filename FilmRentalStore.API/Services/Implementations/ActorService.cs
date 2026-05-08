using FilmRentalStore.API.DTOs.Actor;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _repository;

        public ActorService(IActorRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ActorDTO>> GetAllActorsAsync()
        {
            var actors = await _repository.GetAllActorsAsync();

            return actors.Select(a => new ActorDTO
            {
                ActorId = (short)a.ActorId,
                FirstName = a.FirstName,
                LastName = a.LastName
            });
        }

        public async Task<ActorDTO?> GetActorByIdAsync(short id)
        {
            var actor = await _repository.GetActorByIdAsync(id);

            if (actor == null)
                return null;

            return new ActorDTO
            {
                ActorId = (short)actor.ActorId,
                FirstName = actor.FirstName,
                LastName = actor.LastName
            };
        }

        public async Task<ActorDTO> CreateActorAsync(ActorDTO actorDto)
        {
            var actor = new Actor
            {
                FirstName = actorDto.FirstName,
                LastName = actorDto.LastName
            };

            var createdActor = await _repository.CreateActorAsync(actor);

            actorDto.ActorId = (short)createdActor.ActorId;

            return actorDto;
        }

        public async Task<bool> UpdateActorAsync(short id, ActorDTO actorDto)
        {
            var existingActor = await _repository.GetActorByIdAsync(id);

            if (existingActor == null)
                return false;

            existingActor.FirstName = actorDto.FirstName;
            existingActor.LastName = actorDto.LastName;

            return await _repository.UpdateActorAsync(existingActor);
        }

        public async Task<bool> DeleteActorAsync(short id)
        {
            return await _repository.DeleteActorAsync(id);
        }
    }
}
