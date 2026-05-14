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
    }
}
