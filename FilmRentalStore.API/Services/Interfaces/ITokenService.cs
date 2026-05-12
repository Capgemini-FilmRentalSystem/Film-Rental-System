using FilmRentalStore.API.DTOs.Auth;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ITokenService
    {
        LoginResponseDto GenerateToken(Models.Staff staff);
    }
}