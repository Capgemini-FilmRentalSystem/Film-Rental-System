using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<(IEnumerable<Customer> Customers, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetWithAddressAsync(int id);
        Task<IEnumerable<Customer>> SearchAsync(string? name, string? email);
        Task<IEnumerable<Customer>> GetByStoreIdAsync(int storeId);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
        Task<bool> ExistsAsync(int id);
    }
}