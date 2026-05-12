using FilmRentalStore.API.DTOs.Address;

namespace FilmRentalStore.API.DTOs.Store
{
    public class StoreWithAddressDto
    {
        public int StoreId { get; set; }
        public byte ManagerStaffId { get; set; }
        public string ManagerName { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
        public AddressDto Address { get; set; } = null!;
    }
}
