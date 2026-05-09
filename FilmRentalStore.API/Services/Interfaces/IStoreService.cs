using FilmRentalStore.API.DTOs.Store;

namespace FilmRentalStore.API.Services.Interfaces
{
    public interface IStoreService
    {
        Task<StoreResponseDto> GetStoreByIdAsync(int storeId);
        Task<StoreResponseDto> CreateStoreAsync(StoreCreateDto dto);
        Task<StoreResponseDto> UpdateStoreAsync(int storeId, StoreUpdateDto dto);
    }
}
