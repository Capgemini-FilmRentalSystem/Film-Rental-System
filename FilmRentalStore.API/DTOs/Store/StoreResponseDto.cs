using FilmRentalStore.API.DTOs.Address;
using FilmRentalStore.API.DTOs;

namespace FilmRentalStore.API.DTOs.Store
{
    public class StoreResponseDto
    {
        public int StoreId { get; set; }
        public StaffSummaryDto ManagerStaff { get; set; } = null!;
        public AddressResponseDto Address { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
}
