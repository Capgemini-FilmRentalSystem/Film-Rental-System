using FilmRentalStore.MVC.DTOs.Actor;

namespace FilmRentalStore.MVC.Services.Interfaces
{
    public interface IActorApiService
    {
        Task<List<ActorResponseDto>> GetAllAsync();
        Task<ActorResponseDto?> GetByIdAsync(int id);
        Task<ActorResponseDto?> CreateAsync(ActorRequestDto dto);
        Task<ActorResponseDto?> UpdateAsync(int id, ActorRequestDto dto);
    }
}
