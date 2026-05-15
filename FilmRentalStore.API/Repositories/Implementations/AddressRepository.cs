using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _context;

        public AddressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Addresses
                .AsNoTracking()
                .Include(a => a.City)
                    .ThenInclude(c => c.Country)
                .OrderBy(a => a.AddressId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses
                .Include(a => a.City)
                    .ThenInclude(c => c.Country)
                .FirstOrDefaultAsync(a => a.AddressId == id);
        }

        public async Task<Address> CreateAsync(Address address)
        {
            address.LastUpdate = DateTime.Now;
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> UpdateAsync(Address address)
        {
            address.LastUpdate = DateTime.Now;
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task DeleteAsync(Address address)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}
