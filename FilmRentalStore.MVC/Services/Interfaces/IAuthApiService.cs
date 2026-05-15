using FilmRentalStore.MVC.DTOs.Auth;
using FilmRentalStore.MVC.DTOs.Customers;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IAuthApiService
    {
        Task<LoginResponseDto?> StaffLoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto?> StaffRegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto?> CustomerLoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto?> CustomerRegisterAsync(CustomerRequestDto dto);
    }
}
