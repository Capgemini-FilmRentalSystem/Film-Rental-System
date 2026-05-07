using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
            => await _context.Staff
                .Include(s => s.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .Include(s => s.Store)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Staff?> GetByIdAsync(byte id)
            => await _context.Staff
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id);

        public async Task<Staff?> GetByIdWithDetailsAsync(byte id)
            => await _context.Staff
                .Include(s => s.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .Include(s => s.Store)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id);

        public async Task<IEnumerable<Staff>> GetByStoreIdAsync(int storeId)
            => await _context.Staff
                .Include(s => s.Address)
                .Where(s => s.StoreId == storeId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Staff?> GetByEmailAsync(string email)
            => await _context.Staff
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Email == email);

        public async Task<Staff?> GetByUsernameAsync(string username)
            => await _context.Staff
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Username == username);

        public async Task<Staff> CreateAsync(Staff staff)
        {
            staff.LastUpdate = DateTime.UtcNow;
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<Staff> UpdateAsync(Staff staff)
        {
            staff.LastUpdate = DateTime.UtcNow;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task DeleteAsync(Staff staff)
        {
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(byte id)
            => await _context.Staff.AnyAsync(s => s.StaffId == id);

        public async Task<bool> EmailExistsAsync(string email, byte? excludeId = null)
            => await _context.Staff.AnyAsync(s =>
                s.Email == email && (excludeId == null || s.StaffId != excludeId));

        public async Task<bool> UsernameExistsAsync(string username, byte? excludeId = null)
            => await _context.Staff.AnyAsync(s =>
                s.Username == username && (excludeId == null || s.StaffId != excludeId));

        public async Task UpdateEmailAsync(byte id, string email)
        {
            await _context.Staff
                .Where(s => s.StaffId == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Email, email)
                    .SetProperty(s => s.LastUpdate, DateTime.UtcNow));
        }

        public async Task UpdatePasswordAsync(byte id, string hashedPassword)
        {
            await _context.Staff
                .Where(s => s.StaffId == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Password, hashedPassword)
                    .SetProperty(s => s.LastUpdate, DateTime.UtcNow));
        }

        public async Task UpdateActiveStatusAsync(byte id, bool active)
        {
            await _context.Staff
                .Where(s => s.StaffId == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.Active, active)
                    .SetProperty(s => s.LastUpdate, DateTime.UtcNow));
        }

        public async Task<int> CountByStoreAsync(int storeId)
            => await _context.Staff.CountAsync(s => s.StoreId == storeId);
    }

}
