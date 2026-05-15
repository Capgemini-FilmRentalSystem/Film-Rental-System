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
        public async Task AddAsync(Staff staff)
        {
            await _context.Staff.AddAsync(staff);
        }

        public void Deactivate(Staff staff)
        {
            staff.Active = false;
            staff.LastUpdate = DateTime.Now;

            _context.Entry(staff).State = EntityState.Modified;
        }
        public void Update(Staff staff)
        {
            _context.Entry(staff).State = EntityState.Modified;
        }

        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staff
                .AsNoTracking()
                .Include(s => s.Role)
                .Include(s => s.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .Include(s => s.Store)
                    .ThenInclude(st => st.ManagerStaff)
                        .ThenInclude(ms => ms.Role)
                .OrderBy(s => s.StaffId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetByStoreIdAndRoleAsync(int storeId, string roleTitle)
        {
            return await _context.Staff
                .AsNoTracking()
                .Include(s => s.Role)
                .Include(s => s.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .Include(s => s.Store)
                    .ThenInclude(st => st.ManagerStaff)
                        .ThenInclude(ms => ms.Role)
                .Where(s => s.StoreId == storeId && s.Role != null && s.Role.RoleTitle == roleTitle)
                .OrderBy(s => s.StaffId)
                .ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(byte staffId)
        {
            return await _context.Staff
                .AsNoTracking()
                .Include(s => s.Role)
                .Include(s => s.Address)
                    .ThenInclude(a => a.City)
                        .ThenInclude(c => c.Country)
                .Include(s => s.Store)
                    .ThenInclude(st => st.ManagerStaff)
                        .ThenInclude(ms => ms.Role)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        public async Task<Staff?> GetEntityByIdAsync(byte staffId)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        public async Task<Staff?> GetByUsernameAsync(string username)
        {
            return await _context.Staff
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s => s.Username == username);
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Staff
                .AnyAsync(s => s.Username == username);
        }
        public async Task<bool> IsActiveAsync(byte staffId)
        {
            return await _context.Staff.AnyAsync(s => s.StaffId == staffId && s.Active);
        }

        public async Task<bool> IsAssignedToStore(byte staffid, int storeId)
        {
            return await _context.Staff
                .AnyAsync(s => s.StaffId == staffid && s.StoreId == storeId && s.Active);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
