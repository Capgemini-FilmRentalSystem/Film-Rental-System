using FilmRentalStore.MVC.DTOs.Staff;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class StaffApiService : IStaffApiService
    {
        private readonly IApiClient _apiClient;

        public StaffApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<StaffResponseDto>> GetAllAsync()
        {
            return await _apiClient.GetAsync<List<StaffResponseDto>>(ApiRoutes.Staff) ?? new();
        }

        public async Task<StaffResponseDto?> GetByIdAsync(byte id)
        {
            return await _apiClient.GetAsync<StaffResponseDto>($"{ApiRoutes.Staff}/{id}");
        }

        public async Task<StaffResponseDto?> CreateAsync(StaffCreateRequestDto dto)
        {
            return await _apiClient.PostAsync<StaffCreateRequestDto, StaffResponseDto>(ApiRoutes.Staff, dto);
        }

        public async Task<StaffResponseDto?> UpdateAsync(byte id, StaffUpdateRequestDto dto)
        {
            return await _apiClient.PutAsync<StaffUpdateRequestDto, StaffResponseDto>($"{ApiRoutes.Staff}/{id}", dto);
        }

        public async Task DeactivateAsync(byte id)
        {
            await _apiClient.DeleteAsync($"{ApiRoutes.Staff}/{id}");
        }
    }
}
