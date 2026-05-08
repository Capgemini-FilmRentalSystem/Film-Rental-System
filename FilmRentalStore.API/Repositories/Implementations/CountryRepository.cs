using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;

        public CountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Country?> GetByNameAsync(string countryName)
        {
            return await _context.Countries
                .FirstOrDefaultAsync(c => c.Country1 == countryName);
        }

        public async Task<Country> CreateAsync(Country country)
        {
            country.LastUpdate = DateTime.Now;
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
            return country;
        }
    }
}
