using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface ICityRepository
    {
        Task<City?> GetByNameAndCountryAsync(string cityName, short countryId);
        Task<City> CreateAsync(City city);
    }
}
