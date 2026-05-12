using FilmRentalStore.API.DTOs.Actor;
using FilmRentalStore.API.Services.Implementations;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorsController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActors()
        {
            var actors = await _actorService.GetAllActorsAsync();

            return Ok(actors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActorById(int id)
        {
            var actor = await _actorService.GetActorByIdAsync(id);

            if (actor == null)
                return NotFound();

            return Ok(actor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActor([FromBody] ActorRequestDto actorDto)
        {
            var createdActor = await _actorService.CreateActorAsync(actorDto);

            return CreatedAtAction(
                nameof(GetActorById),
                new { id = createdActor.ActorId },
                createdActor
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActor(int id, ActorRequestDto actorDto)
        {
            var updatedActor = await _actorService.UpdateActorAsync(id, actorDto);

            return Ok(updatedActor);
        }
    }
}
