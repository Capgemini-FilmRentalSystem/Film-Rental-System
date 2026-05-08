using FilmRentalStore.API.Data;
using FilmRentalStore.API.Models;
using FilmRentalStore.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmRentalStore.API.Repositories.Implementations
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<City?> GetByNameAndCountryAsync(string cityName, short countryId)
        {
            return await _context.Cities
                .FirstOrDefaultAsync(c => c.City1 == cityName && c.CountryId == countryId);
        }

        public async Task<City> CreateAsync(City city)
        {
            city.LastUpdate = DateTime.Now;
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return city;
        }
    }
}
