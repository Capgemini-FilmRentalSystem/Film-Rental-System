using FilmRentalStore.MVC.DTOs.Customers;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface ICustomerApiService
    {
        Task<List<CustomerResponseDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<CustomerResponseDto> GetMeAsync();
        Task<CustomerResponseDto> GetByIdAsync(int id);
        Task<List<CustomerResponseDto>> SearchAsync(string? name, string? email);
        Task<List<CustomerResponseDto>> GetActiveAsync();
        Task<List<CustomerResponseDto>> GetByStoreAsync(int storeId);
        Task<CustomerResponseDto> CreateAsync(CustomerRequestDto dto);
        Task<CustomerResponseDto> UpdateAsync(int id, CustomerRequestDto dto);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
    }
}
