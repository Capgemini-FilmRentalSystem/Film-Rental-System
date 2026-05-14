using FilmRentalStore.MVC.DTOs.Address;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IAddressApiService
    {
        Task<List<AddressResponseDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<AddressResponseDto> GetByIdAsync(int id);
        Task<AddressResponseDto> CreateAsync(AddressRequestDto dto);
        Task<AddressResponseDto> UpdateAsync(int id, AddressRequestDto dto);
        Task DeleteAsync(int id);
    }
}
