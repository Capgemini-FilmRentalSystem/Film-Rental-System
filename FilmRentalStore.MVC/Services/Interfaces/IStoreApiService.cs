using FilmRentalStore.MVC.DTOs.Store;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IStoreApiService
    {
        Task<List<StoreResponseDto>> GetAllAsync();
        Task<StoreResponseDto?> GetByIdAsync(int id);
        Task<StoreResponseDto?> CreateAsync(StoreRequestDto dto);
        Task<StoreResponseDto?> UpdateAsync(int id, StoreRequestDto dto);
    }
}
