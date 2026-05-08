using FilmRentalStore.API.DTOs.Actor;
using FilmRentalStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FilmRentalStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly IActorService _service;

        public ActorsController(IActorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActors()
        {
            var actors = await _service.GetAllActorsAsync();

            return Ok(actors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActorById(short id)
        {
            var actor = await _service.GetActorByIdAsync(id);

            if (actor == null)
                return NotFound();

            return Ok(actor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActor(ActorDTO actorDto)
        {
            var createdActor = await _service.CreateActorAsync(actorDto);

            return CreatedAtAction(
                nameof(GetActorById),
                new { id = createdActor.ActorId },
                createdActor
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActor(short id, ActorDTO actorDto)
        {
            var updated = await _service.UpdateActorAsync(id, actorDto);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActor(short id)
        {
            var deleted = await _service.DeleteActorAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
