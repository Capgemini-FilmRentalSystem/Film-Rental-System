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

        public async Task CreateActorAsync(Actor actor)
        {
            await _context.Actors.AddAsync(actor);
        }

        public async Task<bool> ActorExistsAsync(int actorId)
        {
            return await _context.Actors.AnyAsync(a => a.ActorId == actorId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(Actor actor)
        {
            _context.Actors.Update(actor);
        }
    }
}
