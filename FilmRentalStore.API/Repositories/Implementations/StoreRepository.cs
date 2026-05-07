using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Store>> GetAllAsync()
            => await _context.Stores
                .Include(s => s.ManagerStaff)
                .Include(s => s.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Store?> GetByIdAsync(int id)
            => await _context.Stores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StoreId == id);

        public async Task<Store?> GetByIdWithDetailsAsync(int id)
            => await _context.Stores
                .Include(s => s.ManagerStaff)
                .Include(s => s.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .Include(s => s.Staff)
                .Include(s => s.Customers)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StoreId == id);

        public async Task<Store> CreateAsync(Store store)
        {
            store.LastUpdate = DateTime.UtcNow;
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
            return store;
        }

        public async Task<Store> UpdateAsync(Store store)
        {
            store.LastUpdate = DateTime.UtcNow;
            _context.Stores.Update(store);
            await _context.SaveChangesAsync();
            return store;
        }

        public async Task DeleteAsync(Store store)
        {
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Stores.AnyAsync(s => s.StoreId == id);

        public async Task<Staff?> GetStoreManagerAsync(int storeId)
            => await _context.Stores
                .Where(s => s.StoreId == storeId)
                .Include(s => s.ManagerStaff)
                    .ThenInclude(m => m.Address)
                        .ThenInclude(a => a.City)
                .Select(s => s.ManagerStaff)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task UpdateManagerAsync(int storeId, byte managerStaffId)
        {
            await _context.Stores
                .Where(s => s.StoreId == storeId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.ManagerStaffId, managerStaffId)
                    .SetProperty(s => s.LastUpdate, DateTime.UtcNow));
        }

        public async Task UpdateAddressAsync(int storeId, int addressId)
        {
            await _context.Stores
                .Where(s => s.StoreId == storeId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.AddressId, addressId)
                    .SetProperty(s => s.LastUpdate, DateTime.UtcNow));
        }

        public async Task<IEnumerable<Staff>> GetStaffByStoreAsync(int storeId)
            => await _context.Staff
                .Where(s => s.StoreId == storeId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Customer>> GetCustomersByStoreAsync(int storeId)
            => await _context.Customers
                .Where(c => c.StoreId == storeId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<StoreSalesSummary>> GetSalesByStoreAsync()
            => await _context.Payments
                .Join(_context.Rentals, p => p.RentalId, r => r.RentalId, (p, r) => new { p, r })
                .Join(_context.Inventories, pr => pr.r.InventoryId, i => i.InventoryId, (pr, i) => new { pr.p, i })
                .Join(_context.Stores, pri => pri.i.StoreId, s => s.StoreId, (pri, s) => new { pri.p, s })
                .Join(_context.Addresses, ps => ps.s.AddressId, a => a.AddressId, (ps, a) => new { ps.p, ps.s, a })
                .Join(_context.Cities, psa => psa.a.CityId, c => c.CityId, (psa, c) => new { psa.p, psa.s, c })
                .Join(_context.Countries, psac => psac.c.CountryId, cy => cy.CountryId, (psac, cy) => new { psac.p, psac.s, psac.c, cy })
                .Join(_context.Staff, psacc => psacc.s.ManagerStaffId, m => m.StaffId, (psacc, m) => new { psacc.p, psacc.s, psacc.c, psacc.cy, m })
                .GroupBy(x => new { x.s.StoreId, City = x.c.City1, Country = x.cy.Country1, ManagerFirst = x.m.FirstName, ManagerLast = x.m.LastName })
                .Select(g => new StoreSalesSummary
                {
                    StoreId = g.Key.StoreId,
                    Store = $"{g.Key.City},{g.Key.Country}",
                    Manager = $"{g.Key.ManagerFirst} {g.Key.ManagerLast}",
                    TotalSales = g.Sum(x => x.p.Amount)
                })
                .AsNoTracking()
                .ToListAsync();

        public async Task<int> GetInventoryCountAsync(int storeId)
            => await _context.Inventories.CountAsync(i => i.StoreId == storeId);
    }

}
