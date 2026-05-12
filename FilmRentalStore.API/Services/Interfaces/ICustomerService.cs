using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.DTOs.Customers;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ICustomerService
    {
        const int DefaultPage = 1;
        const int DefaultPageSize = 10;
        const int MaxPageSize = 100;

        /// <summary>Returns a paged customer result; page and pageSize must be within the defined pagination bounds.</summary>
        Task<IEnumerable<CustomerResponseDto>> GetAllAsync(int page, int pageSize);
        Task<CustomerResponseDto> GetByIdAsync(int id);
        /// <summary>Returns matching customers; throws NotFoundException when no customers match.</summary>
        Task<IEnumerable<CustomerResponseDto>> SearchAsync(string? name, string? email);
        /// <summary>Returns customers for a store; throws NotFoundException when the store or matching customers do not exist.</summary>
        Task<IEnumerable<CustomerResponseDto>> GetByStoreIdAsync(int storeId);
        /// <summary>Returns active customers; throws NotFoundException when no active customers exist.</summary>
        Task<IEnumerable<CustomerResponseDto>> GetActiveCustomersAsync();
        Task<CustomerResponseDto> CreateAsync(CustomerRequestDto dto);
        Task<CustomerResponseDto> UpdateAsync(int id, CustomerRequestDto dto);
        Task DeleteAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task<AddressResponseDto> GetCustomerAddressAsync(int id);
    }
}
