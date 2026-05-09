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

        public async Task AddAsync(Store store)
        {
            await _context.Stores.AddAsync(store);
        }

        public async Task<bool> AddressExistsAsync(int addressId)
        {
            return await _context.Addresses
                .AnyAsync(s => s.AddressId == addressId);
        }

        public async Task<Store?> GetByIdAsync(int storeId)
        {
            return await _context.Stores
                .Include(s => s.ManagerStaff)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);
        }

        public async Task<bool> ManagerStaffExistsAsync(int managerStaffId)
        {
            return await _context.Staff
                .AnyAsync(s => s.StaffId == managerStaffId && s.Active);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> StoreExists(int storeId)
        {
            return await _context.Stores
                .AnyAsync(s => s.StoreId == storeId);
        }

        public void Update(Store store)
        {
            _context.Stores.Update(store);
        }
    }
}
