using FilmRentalStore.API.DTOs.Address;

namespace FilmRentalStore.API.DTOs.Store
{
    public class StoreRequestDto
    {
        public byte ManagerStaffId { get; set; }
        public int AddressId { get; set; }
        public AddressDto? Address { get; set; }
    }
}
