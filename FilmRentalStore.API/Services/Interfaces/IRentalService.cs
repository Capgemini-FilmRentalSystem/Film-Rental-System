using FilmRentalStore.API.DTOs.Rental;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalResponseDto>> GetAllRentalsAsync(int page, int pageSize);

        Task<RentalResponseDto> GetRentalByIdAsync(int rentalId);

        Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto rentalDto);

        Task<RentalResponseDto> ReturnRentalAsync(int rentalId, ReturnRentalDto rentalReturnDto);
    }
}