using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Inventories
                .Include(i => i.Film)
                .Include(i => i.Store)
                .Include(i => i.Rentals)
                .ToListAsync();
        }

        public async Task<Inventory?> GetByIdAsync(int inventoryId)
        {
            return await _context.Inventories
                .Include(i => i.Film)
                .Include(i => i.Store)
                .Include(i => i.Rentals)
                .FirstOrDefaultAsync(i => i.InventoryId == inventoryId);
        }

        public async Task<bool> InventoryExistsAsync(int inventoryId)
        {
            return await _context.Inventories
                .AnyAsync(i => i.InventoryId == inventoryId);
        }

        public async Task AddAsync(Inventory inventory)
        {
            inventory.LastUpdate = DateTime.Now;

            await _context.Inventories.AddAsync(inventory);
        }

        public void Update(Inventory inventory)
        {
            inventory.LastUpdate = DateTime.Now;

            _context.Inventories.Update(inventory);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}