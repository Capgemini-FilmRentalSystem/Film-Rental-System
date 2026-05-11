using FilmRentalStore.API.DTOs.Rental;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IRentalService
    {
        const int DefaultPage = 1;
        const int DefaultPageSize = 10;
        const int MaxPageSize = 100;

        /// <summary>Returns a paged rental result; throws NotFoundException when no rentals exist.</summary>
        Task<IEnumerable<RentalResponseDto>> GetAllRentalsAsync(int page, int pageSize);

        Task<RentalResponseDto> GetRentalByIdAsync(int rentalId);

        Task<RentalResponseDto> CreateRentalAsync(CreateRentalDto rentalDto);

        Task<RentalResponseDto> ReturnRentalAsync(int rentalId, ReturnRentalDto rentalReturnDto);
    }
}
