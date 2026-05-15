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

        public async Task<(IEnumerable<Inventory> Inventory, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var totalCount = await _context.Inventories.CountAsync();

            var inventories = await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Film)
                .Include(i => i.Store)
                    .ThenInclude(s => s.ManagerStaff)
                        .ThenInclude(ms => ms.Role)
                .Include(i => i.Rentals.Where(r => r.ReturnDate == null))
                .OrderBy(i => i.InventoryId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (inventories, totalCount);
        }

        public async Task<Inventory?> GetByIdAsync(int inventoryId)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Film)
                .Include(i => i.Store)
                    .ThenInclude(s => s.ManagerStaff)
                        .ThenInclude(ms => ms.Role)
                .Include(i => i.Rentals.Where(r => r.ReturnDate == null))
                .FirstOrDefaultAsync(i => i.InventoryId == inventoryId);
        }

        public async Task<Inventory?> GetEntityByIdAsync(int inventoryId)
        {
            return await _context.Inventories
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

            _context.Entry(inventory).State = EntityState.Modified;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
