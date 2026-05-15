using FilmRentalStore.MVC.DTOs.Rental;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IRentalApiService
    {
        Task<List<RentalResponseDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<List<RentalResponseDto>> GetMineAsync(int page = 1, int pageSize = 10);
        Task<RentalResponseDto?> GetByIdAsync(int id);
        Task<RentalResponseDto?> GetMineByIdAsync(int id);
        Task<RentalResponseDto?> CreateAsync(RentalRequestDto dto);
        Task<RentalResponseDto?> ReturnAsync(int id, RentalReturnRequestDto dto);
    }
}
