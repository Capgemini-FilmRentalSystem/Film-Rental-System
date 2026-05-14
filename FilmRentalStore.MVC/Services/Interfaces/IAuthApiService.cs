using FilmRentalStore.MVC.DTOs.Auth;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IAuthApiService
    {
        Task<LoginResponseDto?> StaffLoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto?> CustomerLoginAsync(LoginRequestDto dto);
    }
}
