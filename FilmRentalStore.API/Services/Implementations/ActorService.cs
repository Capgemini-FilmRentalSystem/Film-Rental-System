using AutoMapper;
using FilmRentalStore.API.DTOs.Actor;
using FilmRentalStore.API.Exceptions;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Interfaces;

namespace FilmRentalStore.API.Services.Implementations
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorRepository;
        private readonly IMapper _mapper;

        public ActorService(IActorRepository actorRepository, IMapper mapper)
        {
            _actorRepository = actorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ActorResponseDto>> GetAllActorsAsync()
        {
            var actors = await _actorRepository.GetAllActorsAsync();

            if (actors is null || !actors.Any())
                throw new NotFoundException("No actors found.");

            return _mapper.Map<IEnumerable<ActorResponseDto>>(actors);
        }

        public async Task<ActorResponseDto> GetActorByIdAsync(int id)
        {
            var actor = await _actorRepository.GetActorByIdAsync(id);

            if (actor == null)
                throw new NotFoundException("Actor not found.");

            return _mapper.Map<ActorResponseDto>(actor);
        }

        public async Task<ActorResponseDto> CreateActorAsync(ActorDTO actorDto)
        {
            if (actorDto == null)
                throw new BadRequestException("Actor data is required.");

            var actor = _mapper.Map<Actor>(actorDto);

            actor.LastUpdate = DateTime.Now;

            await _actorRepository.CreateActorAsync(actor);
            await _actorRepository.SaveChangesAsync();

            var createdActor = await _actorRepository.GetActorByIdAsync((int)actor.ActorId);

            if (createdActor == null)
                throw new NotFoundException("Created actor record not found.");

            return _mapper.Map<ActorResponseDto>(createdActor);
        }

        public async Task<ActorResponseDto> UpdateActorAsync(int id, ActorDTO actorDto)
        {
            if (actorDto == null)
                throw new BadRequestException("Actor data is required.");

            var actor = await _actorRepository.GetActorByIdAsync(id);

            if (actor == null)
                throw new NotFoundException("Actor not found.");

            _mapper.Map(actorDto, actor);

            actor.LastUpdate = DateTime.Now;

            _actorRepository.Update(actor);
            await _actorRepository.SaveChangesAsync();

            var updatedActor = await _actorRepository.GetActorByIdAsync(id);

            if (updatedActor == null)
                throw new NotFoundException("Updated actor record not found.");

            return _mapper.Map<ActorResponseDto>(updatedActor);
        }
    }
}
