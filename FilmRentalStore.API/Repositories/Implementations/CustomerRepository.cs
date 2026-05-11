using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Customer> Customers, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var query = _context.Customers.AsQueryable();
            var totalCount = await query.CountAsync();
            var customers = await query
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (customers, totalCount);
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer?> GetWithAddressAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Address)
                .ThenInclude(a => a.City)
                .ThenInclude(ci => ci.Country)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string? name, string? email)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(c =>
                    c.FirstName.Contains(name) ||
                    c.LastName.Contains(name) ||
                    (c.FirstName + " " + c.LastName).Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(c => c.Email != null && c.Email.Contains(email));
            }

            return await query
                .OrderBy(c => c.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetByStoreIdAsync(int storeId)
        {
            return await _context.Customers
                .Where(c => c.StoreId == storeId)
                .OrderBy(c => c.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.Active == "Y")
                .OrderBy(c => c.LastName)
                .ToListAsync();
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            customer.LastUpdate = DateTime.Now;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == id);
        }
    }
}