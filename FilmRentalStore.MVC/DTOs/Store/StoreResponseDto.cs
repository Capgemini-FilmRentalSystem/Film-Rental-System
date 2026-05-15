using FilmRentalStore.MVC.DTOs.Address;
using FilmRentalStore.MVC.DTOs;

namespace FilmRentalStore.MVC.DTOs.Store
{
    public class StoreResponseDto
    {
        public int StoreId { get; set; }
        public byte ManagerStaffId { get; set; }
        public int AddressId { get; set; }
        public StaffSummaryDto ManagerStaff { get; set; } = null!;
        public AddressResponseDto Address { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
    }
}
