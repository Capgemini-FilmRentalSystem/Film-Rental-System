using FilmRentalStore.API.DTOs.Auth;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
