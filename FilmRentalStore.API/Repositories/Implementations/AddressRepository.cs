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

        public async Task<Address?> GetByIdAsync(int id)
        {
            return await _context.Addresses
                .Include(a => a.City)
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
    }
}
