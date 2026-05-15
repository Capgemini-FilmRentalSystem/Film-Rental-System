using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync(int page, int pageSize);
        Task<Address?> GetByIdAsync(int id);
        Task<Address> CreateAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task DeleteAsync(Address address);
    }
}
