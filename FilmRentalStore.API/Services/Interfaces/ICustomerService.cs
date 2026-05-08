using FilmRentalStore.API.DTOs.Customers;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllAsync(int page, int pageSize);
        Task<CustomerResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<CustomerResponseDto>> SearchAsync(string? name, string? email);
        Task<IEnumerable<CustomerResponseDto>> GetByStoreIdAsync(int storeId);
        Task<IEnumerable<CustomerResponseDto>> GetActiveCustomersAsync();
        Task<CustomerResponseDto> CreateAsync(CustomerCreateDto dto);
        Task<CustomerResponseDto> UpdateAsync(int id, CustomerUpdateDto dto);
        Task DeleteAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<CustomerAddressDto> GetCustomerAddressAsync(int id);
    }
}