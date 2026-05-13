using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.DTOs.Customers;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginStaffAsync(LoginRequestDto dto);
        Task<LoginResponseDto> LoginCustomerAsync(LoginRequestDto dto);
        Task<LoginResponseDto> RegisterStaffAsync(RegisterRequestDto dto);
        Task<LoginResponseDto> RegisterCustomerAsync(CustomerRequestDto dto);
    }
}
