using FilmRentalStore.MVC.DTOs.Actor;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class ActorApiService : IActorApiService
    {
        private readonly IApiClient _apiClient;

        public ActorApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<ActorResponseDto>> GetAllAsync()
        {
            return await _apiClient.GetAsync<List<ActorResponseDto>>(ApiRoutes.Actors) ?? new();
        }

        public async Task<ActorResponseDto?> GetByIdAsync(int id)
        {
            return await _apiClient.GetAsync<ActorResponseDto>($"{ApiRoutes.Actors}/{id}");
        }

        public async Task<ActorResponseDto?> CreateAsync(ActorRequestDto dto)
        {
            return await _apiClient.PostAsync<ActorRequestDto, ActorResponseDto>(ApiRoutes.Actors, dto);
        }

        public async Task<ActorResponseDto?> UpdateAsync(int id, ActorRequestDto dto)
        {
            return await _apiClient.PutAsync<ActorRequestDto, ActorResponseDto>($"{ApiRoutes.Actors}/{id}", dto);
        }
    }
}
