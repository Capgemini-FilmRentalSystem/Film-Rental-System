using FilmRentalStore.MVC.DTOs.Auth;
using FilmRentalStore.MVC.DTOs.Customers;
using FilmRentalStore.MVC.Helpers;
using FilmRentalStore.MVC.Services.Interfaces;

namespace FilmRentalStore.MVC.Services.Implementations
{
    public class AuthApiService : IAuthApiService
    {
        private readonly IApiClient _apiClient;

        public AuthApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<LoginResponseDto?> StaffLoginAsync(LoginRequestDto dto)
        {
            return await _apiClient.PostAsync<LoginRequestDto, LoginResponseDto>(
                ApiRoutes.StaffLogin,
                dto);
        }

        public async Task<LoginResponseDto?> CustomerLoginAsync(LoginRequestDto dto)
        {
            return await _apiClient.PostAsync<LoginRequestDto, LoginResponseDto>(
                ApiRoutes.CustomerLogin,
                dto);
        }

        public async Task<LoginResponseDto?> CustomerRegisterAsync(CustomerRequestDto dto)
        {
            return await _apiClient.PostAsync<CustomerRequestDto, LoginResponseDto>(
                ApiRoutes.CustomerRegister,
                dto);
        }
    }
}
