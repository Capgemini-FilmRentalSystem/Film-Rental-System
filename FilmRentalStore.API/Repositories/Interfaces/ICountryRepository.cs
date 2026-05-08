using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        Task<Country?> GetByNameAsync(string countryName);
        Task<Country> CreateAsync(Country country);
    }
}
