using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class RentalRepository : IRentalRepository
    {
        private readonly ApplicationDbContext _context;

        public RentalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rental>> GetAllAsync()
        {
            return await _context.Rentals
                .Include(r => r.Inventory)
                    .ThenInclude(i => i.Film)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .ToListAsync();
        }

        public async Task<Rental?> GetByIdAsync(int rentalId)
        {
            return await _context.Rentals
                .Include(r => r.Inventory)
                    .ThenInclude(i => i.Film)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId);
        }

        public async Task<bool> RentalExistsAsync(int rentalId)
        {
            return await _context.Rentals
                .AnyAsync(r => r.RentalId == rentalId);
        }

        public async Task<bool> IsInventoryCurrentlyRentedAsync(int inventoryId)
        {
            return await _context.Rentals
                .AnyAsync(r => r.InventoryId == inventoryId && r.ReturnDate == null);
        }

        public async Task AddAsync(Rental rental)
        {
            rental.RentalDate = DateTime.Now;
            rental.LastUpdate = DateTime.Now;

            await _context.Rentals.AddAsync(rental);
        }

        public void Update(Rental rental)
        {
            rental.LastUpdate = DateTime.Now;

            _context.Rentals.Update(rental);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}