using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressResponseDto>> GetAllAsync(int page, int pageSize);
        Task<AddressResponseDto> GetByIdAsync(int id);
        Task<AddressResponseDto> CreateAsync(AddressRequestDto dto);
        Task<AddressResponseDto> UpdateAsync(int id, AddressRequestDto dto);
        Task DeleteAsync(int id);
        Task<Address> CreateAddressAsync(AddressDto dto);
        Task<Address> UpdateAddressAsync(int addressId, AddressDto dto);
    }
}
