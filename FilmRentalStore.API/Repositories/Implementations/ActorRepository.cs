using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class ActorRepository : IActorRepository

    {
        private readonly ApplicationDbContext _context;

        public ActorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Actor>> GetAllActorsAsync()
        {
            return await _context.Actors.ToListAsync();
        }

        public async Task<Actor?> GetActorByIdAsync(short id)
        {
            return await _context.Actors.FindAsync(id);
        }

        public async Task<Actor> CreateActorAsync(Actor actor)
        {
            _context.Actors.Add(actor);

            await _context.SaveChangesAsync();

            return actor;
        }

        public async Task<bool> UpdateActorAsync(Actor actor)
        {
            _context.Actors.Update(actor);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteActorAsync(short id)
        {
            var actor = await _context.Actors.FindAsync(id);

            if (actor == null)
                return false;

            _context.Actors.Remove(actor);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
