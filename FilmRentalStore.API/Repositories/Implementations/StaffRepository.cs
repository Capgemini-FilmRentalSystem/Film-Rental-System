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

            _context.Staff.Update(staff);
        }
        public void Update(Staff staff)
        {
            _context.Staff.Update(staff);
        }

        public async Task<Staff> GetByIdAsync(byte staffId)
        {
            return await _context.Staff
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }
        public async Task<Staff> GetByUsernameAsync(string username)
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
