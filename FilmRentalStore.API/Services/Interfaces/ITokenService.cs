using FilmRentalStore.API.DTOs.Auth;
using FilmRentalStore.API.Models;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface ITokenService
    {
        LoginResponseDto GenerateToken(Staff staff);
        LoginResponseDto GenerateToken(Customer customer);
    }
}
